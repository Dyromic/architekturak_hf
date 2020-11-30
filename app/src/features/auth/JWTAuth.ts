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
import { useEffect } from 'react';

export const useJWTAuth = () => {

    const microServiceRouter = useMicroServiceRouter();
    const dispatch = useAppDispatch();
    const history = useHistory();
    const { user, authenticated } = useSelector( (state: RootState) => state.auth )

    const convertJWTUserToUser = (jwtuserclaims: JWTUserClaims) => {
      const user: User = {
        userId: jwtuserclaims.nameid, 
        email: jwtuserclaims.email, 
        firstName: jwtuserclaims.family_name, 
        lastName: jwtuserclaims.given_name
      };
      return user;
    };

    useEffect(() => {

      const localStorageToken = localStorage.getItem("JWTToken")
      if (localStorageToken !== null && localStorageToken !== undefined) {
        const jwtuserclaims = jwtDecode<JWTUserClaims>(localStorageToken);
        const user = convertJWTUserToUser(jwtuserclaims);
        axios.defaults.headers.common['Authorization'] = `Bearer ${localStorageToken}`;
        dispatch(setAuthentication(user));
      }

    }, [dispatch]);


    const Login = (email: string, password: string, remember: boolean) => {
       
      return async (dispatch: AppDispatch) => {

        const response = await microServiceRouter.post('auth', 'login', {
          Email: email,
          Password: password
        });
        
        if (response === undefined || response.status !== 200) return;

        const token = response.data.token;
        const jwtuserclaims = jwtDecode<JWTUserClaims>(token);
        const user = convertJWTUserToUser(jwtuserclaims);

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
        const user = convertJWTUserToUser(jwtuserclaims);

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