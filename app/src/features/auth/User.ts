/*export enum UserRole {
    User, Admin
};*/

export interface User {
    userId: string,
    email: string,
    firstName: string,
    lastName: string
    //role: UserRole
};
export interface JWTUserClaims {
    nameid: string,
    email: string,
    family_name: string,
    given_name: string
};