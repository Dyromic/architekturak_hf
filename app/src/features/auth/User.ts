export enum UserRole {
    User, Admin
};

export interface User {
    userId: number,
    firstName: string,
    lastName: string,
    email: string,
    role: UserRole
};