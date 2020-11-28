//import { useEffect } from "react";
import { useSelector } from 'react-redux'

import axios from 'axios'

import { useAppDispatch, RootState, AppDispatch } from '../../reducers/store'
import { LoginOnEndpoint, RegisterOnEndpoint, setAuthentication, removeAuthentication } from './authSlice'
import { User } from './User'

import { useMicroService as useMicroServiceRouter } from '../microservice/useMicroServiceRouter'

export const useJWTAuth = () => {

    const microServiceRouter = useMicroServiceRouter();
    const authEndpoint = microServiceRouter.getServiceEndpoint('auth');

    const dispatch = useAppDispatch();
    const { user, authenticated } = useSelector( (state: RootState) => state.auth )

    const Login = (email: string, password: string) => {
        
      microServiceRouter.IterateServiceEndpoints('auth',
        endpoint => dispatch(LoginOnEndpoint(endpoint, email, password))
      );
      
    };

    const Logout = () => {
      delete axios.defaults.headers.common['Authorization'];
      dispatch(removeAuthentication(user));
    };

    const Register = (email: string, password: string, firstName: string, lastName: string) => {
  
      microServiceRouter.IterateServiceEndpoints('auth',
        endpoint => dispatch(RegisterOnEndpoint(endpoint, email, password, firstName, lastName))
      );

    };

    const isAuthenticated = () => {
        return authenticated;
    }

    return {
        user,
        isAuthenticated,
        Login,
        Logout,
        Register
    };

};