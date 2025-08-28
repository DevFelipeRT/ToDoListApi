import LoginForm from "@/features/auth/components/LoginForm"
import { normalizeStep, authSteps } from "@/features/auth/lib/routes"

type PageProps = {
  params: { step?: string[] }
  searchParams: Record<string, string | string[] | undefined>
}

export default function AuthPage({ params, searchParams }: PageProps) {

  const step = normalizeStep(params.step)

  switch (step) {
    case authSteps.login:
      return <LoginForm />

    case authSteps.register:
      return <div>RegisterForm entra aqui</div>

    case authSteps.resetRequest:
      return <div>ResetRequestForm entra aqui</div>

    case authSteps.resetConfirm:
      return (
        <div>
          ResetPasswordForm entra aqui (token: {searchParams.token})
        </div>
      )

    case authSteps.verifyEmail:
      return (
        <div>
          VerifyEmailPanel entra aqui (token: {searchParams.token})
        </div>
      )

    case authSteps.twofactor:
      return <div>TwoFactorForm entra aqui</div>

    default:
      return <div>LoginForm entra aqui</div>
  }
}
