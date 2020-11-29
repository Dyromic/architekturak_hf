//import { useEffect } from "react";
import { useSelector } from 'react-redux'
import { useHistory } from "react-router-dom";

import axios from 'axios'
import jwtDecode from 'jwt-decode'

import { useAppDispatch, RootState, AppDispatch } from '../../reducers/store'
import { setAuthentication, removeAuthentication } from './authSlice'

import { useMicroService as useMicroServiceRouter } from '../microservice/useMicroServiceRouter'

import routes from './../../routes'
import { JWTUserClaims, User } from './User';

export const useJWTAuth = () => {

    const microServiceRouter = useMicroServiceRouter();
    const dispatch = useAppDispatch();
    const history = useHistory();
    const { user, authenticated } = useSelector( (state: RootState) => state.auth )

    const Login = (email: string, password: string, remember: boolean) => {
       
      return async (dispatch: AppDispatch) => {

        const response = await microServiceRouter.post('auth', 'login', {
          Email: email,
          Password: password
        });
        
        if (response === undefined || response.status !== 200) return;

        const token = response.data.token;
        const jwtuserclaims = jwtDecode<JWTUserClaims>(token);
        const user: User = {
          userId: jwtuserclaims.nameid, 
          email: jwtuserclaims.email, 
          firstName: jwtuserclaims.family_name, 
          lastName: jwtuserclaims.given_name
        };

        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

        if (remember) {
          window.localStorage.setItem("JWTToken", token);
        }
    
        dispatch(setAuthentication(user));

        history.push(routes.dashboard);

      };

    };

    const Logout = () => {
      window.localStorage.clear();
      delete axios.defaults.headers.common['Authorization'];
      dispatch(removeAuthentication(user));
    };

    const Register = (email: string, password: string, firstName: string, lastName: string) => {
  
      return async (dispatch: AppDispatch) => {

        const response = await microServiceRouter.post('auth', 'register',  {
          Email: email,
          FirstName: firstName,
          LastName: lastName,
          Password: password
        });


        if (response === undefined || response.status !== 200) return;

        const token = response.data.token;
        const jwtuserclaims = jwtDecode<JWTUserClaims>(token);
        const user: User = {
          userId: jwtuserclaims.nameid, 
          email: jwtuserclaims.email, 
          firstName: jwtuserclaims.family_name, 
          lastName: jwtuserclaims.given_name
        };

        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  
        dispatch(setAuthentication(user));

      };

    };

    return {
        user,
        authenticated,
        Login,
        Logout,
        Register
    };

};