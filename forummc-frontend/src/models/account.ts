interface Account {
    id: number,
    userName: string | null,
    role: AccountRoles,
    createdAt: number,
    updatedAt: number,
}

enum AccountRoles {
    User,
    Moderator,
    Admin,
}
