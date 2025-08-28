"use client"

import { useState, useTransition } from "react"
import { loginAction } from "@/features/auth/actions/login"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
} from "@/components/ui/card"

export default function LoginForm() {
  // Estado para mensagem de erro
  const [formError, setFormError] = useState<string | null>(null)

  // Controle de loading
  const [pending, startTransition] = useTransition()

  return (
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Entrar</CardTitle>
      </CardHeader>
      <CardContent>
        <form
          action={(formData) =>
            startTransition(async () => {
              setFormError(null)
              const result = await loginAction(formData)
              console.log("Resposta da API:", result.debug)
              if (result?.ok === false) {
                setFormError(result.message ?? "Erro ao entrar")
              }
            })
          }
          className="grid gap-4"
        >
          {/* Campo de e-mail */}
          <div className="grid gap-1">
            <Label htmlFor="email">E-mail</Label>
            <Input
              id="email"
              name="email"
              type="email"
              autoComplete="email"
              required
            />
          </div>

          {/* Campo de senha */}
          <div className="grid gap-1">
            <Label htmlFor="password">Senha</Label>
            <Input
              id="password"
              name="password"
              type="password"
              autoComplete="current-password"
              required
            />
          </div>

          {/* Mensagem de erro */}
          {formError && (
            <p className="text-sm text-red-600">{formError}</p>
          )}

          {/* Botão */}
          <Button type="submit" disabled={pending}>
            {pending ? "Entrando…" : "Entrar"}
          </Button>
        </form>
      </CardContent>
    </Card>
  )
}
