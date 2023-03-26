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

interface Message {

    id: number,
    bodyText: string,
    account: Account,
    goesLive: number,
    isHidden: boolean,
    inReplyTo: Message,
    createdAt: number,
    updatedAt: number,
}

interface Topic {

    id: number,
    createdBy: Account,
    category: Category,
    isHidden: boolean,
    createdAt: number,
    updatedAt: number,
}

interface Category {
    id: number,
    title: string,
    createdAt: number,
    updatedAt: number,
}

