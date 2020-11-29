import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { User } from './User'

type AuthState = {
    user?: User,
    authenticated: boolean
};

const initialAuthState: AuthState = {
    user: undefined,
    authenticated: false
};

const authSlice = createSlice({
  name: 'auth',
  initialState: initialAuthState,
  reducers: {

    setAuthentication: (state, action: PayloadAction<User>) => {
        const user = action.payload;
        state.authenticated = true;
        state.user = user;
    },
    removeAuthentication: (state, action: PayloadAction<User>) => {
        if (state.authenticated === true) {
            state.authenticated = false;
            state.user = undefined;
        }
    }

  }
})

export const { setAuthentication, removeAuthentication } = authSlice.actions

export default authSlice.reducer
