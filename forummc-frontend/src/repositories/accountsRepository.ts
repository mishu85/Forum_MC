class AccountRepository {
	readonly networkClient: any;

	constructor(networkClient: any) {
		this.networkClient = networkClient;
	}

	async login(userName: string, password: string): Promise<Account> {
		var account: Account = {
			id: 0,
			userName: null,
			role: AccountRoles.User,
			createdAt: Date.now(),
			updatedAt: Date.now(),
		};
		return account;
	}
}