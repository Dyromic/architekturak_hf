import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import jwt_decode from 'jwt-decode'
import axios from 'axios'

import { User, JWTUserClaims } from './User'
import {AppDispatch} from './../../reducers/store'

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

export const LoginOnEndpoint = (authEndpoint: string, email: string, password: string) => {

  return async (dispatch: AppDispatch) => {

    const response = await axios.post(`${authEndpoint}/login`, {
      Email: email,
      Password: password
    });

    if (response === undefined || response.status !== 200) return response.status;

    window.localStorage.setItem("JWTToken", response.data);
    const jwtuserclaims = jwt_decode<JWTUserClaims>(response.data);
    axios.defaults.headers.common['Authorization'] = `Bearer ${response.data}`;
    const user: User = {
      userId: jwtuserclaims.nameid, 
      email: jwtuserclaims.email, 
      firstName: jwtuserclaims.family_name, 
      lastName: jwtuserclaims.given_name
    };

    dispatch(setAuthentication(user));
    return response.status;

  };
  
};

export const RegisterOnEndpoint = (authEndpoint: string, email: string, password: string, firstName: string, lastName: string) => {

  return async (dispatch: AppDispatch) => {

    const response = await axios.post(`${authEndpoint}/register`, {
      Email: email,
      FirstName: firstName,
      LastName: lastName,
      Password: password
    });
    
    if (response === undefined || response.status !== 200) return response.status;

    const jwtuserclaims = jwt_decode<JWTUserClaims>(response.data);
    window.localStorage.setItem("JWTToken", response.data);
    axios.defaults.headers.common['Authorization'] = `Bearer ${response.data}`;
    const user: User = {
      userId: jwtuserclaims.nameid, 
      email: jwtuserclaims.email, 
      firstName: jwtuserclaims.family_name, 
      lastName: jwtuserclaims.given_name
    };
    dispatch(setAuthentication(user)); 
    return response.status;
    
  };
  
  
};

export default authSlice.reducer
