export interface SecurityToken {
    access_token: string
    refresh_token: string
    token_type: string
    expires_in: number
    expires_at: number
    isLoggedIn: boolean
}