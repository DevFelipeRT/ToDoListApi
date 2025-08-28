export const authSteps = {
  login: "",
  register: "register",
  resetRequest: "reset/request",
  resetConfirm: "reset/confirm",
  verifyEmail: "verify-email",
  twofactor: "2fa",
} as const

export type AuthStep = typeof authSteps[keyof typeof authSteps]

export function normalizeStep(segments?: string[]): AuthStep {
  const path = (segments ?? []).join("/").toLowerCase()

  switch (path) {
    case "":
      return authSteps.login
    case authSteps.register:
      return authSteps.register
    case authSteps.resetRequest:
      return authSteps.resetRequest
    case authSteps.resetConfirm:
      return authSteps.resetConfirm
    case authSteps.verifyEmail:
      return authSteps.verifyEmail
    case authSteps.twofactor:
      return authSteps.twofactor
    default:
      return authSteps.login
  }
}
