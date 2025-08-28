"use server";

import { resumePluginState } from "next/dist/build/build-context";
import { cookies } from "next/headers";
import { z } from "zod";

const schema = z.object({
  email: z.email(),
  password: z.string().min(6).max(100),
});

type LoginResult = { ok: true; debug?: unknown } | { ok: false; message: string; debug?: unknown };

interface DotNetLoginSuccess {
  token: string;
  refreshToken?: string;
  expiresIn?: number;
  user?: unknown;
}

export async function loginAction(formData: FormData): Promise<LoginResult> {
  const parsed = schema.safeParse({
    email: formData.get("email"),
    password: formData.get("password"),
  });

  if (!parsed.success) {
    const msg =
      parsed.error.issues.map((i) => i.message).join("; ") ||
      "Dados inválidos.";
    return { ok: false, message: msg };
  }

  // Base de servidor (URL interna do Docker). Evite NEXT_PUBLIC_... em server actions.
  const API_BASE = (process.env.API_BASE_URL ?? "http://backend:8080").trim();
  if (!API_BASE) {
    return { ok: false, message: "Configuração ausente: API_BASE_URL." };
  }

  const controller = new AbortController();
  const id = setTimeout(() => controller.abort(), 15_000);

  let resp: Response;
  try {
    resp = await fetch(`${API_BASE}/api/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify({
        email: parsed.data.email,
        password: parsed.data.password,
      }),
      signal: controller.signal,
      cache: "no-store",
    });
  } catch {
    clearTimeout(id);
    return { ok: false, message: "Falha de rede ou tempo esgotado." };
  } finally {
    clearTimeout(id);
  }

  if (!resp.ok) {
    try {
      const data = await resp.json();
      const message: string =
        data?.message || data?.error || `Falha no login (${resp.status}).`;
      return { ok: false, message };
    } catch {
      return { ok: false, message: `Falha no login (${resp.status}).` };
    }
  }

  let data: DotNetLoginSuccess;
  try {
    data = (await resp.json()) as DotNetLoginSuccess;
    console.log("Resposta da API:", data)
  } catch {
    return { ok: false, message: "Resposta inválida da API." };
  }

  if (!data?.token) {
    return { ok: false, message: "Token não retornado pela API." };
  }

  const ttl =
    typeof data.expiresIn === "number" && data.expiresIn > 0
      ? data.expiresIn
      : Number(process.env.DEFAULT_ACCESS_TOKEN_TTL || 900);

  const secure = String(process.env.COOKIE_SECURE).toLowerCase() === "true";

  const cookieStore = await cookies();

  cookieStore.set("access_token", data.token, {
    httpOnly: true,
    secure,
    sameSite: "lax",
    path: "/",
    maxAge: ttl,
  });

  if (data.refreshToken) {
    cookieStore.set("refresh_token", data.refreshToken, {
      httpOnly: true,
      secure,
      sameSite: "lax",
      path: "/",
      maxAge: 60 * 60 * 24 * 7,
    });
  }

  return { ok: true, debug: data };
}
