//import { useEffect } from "react";
import { useSelector } from 'react-redux'

import axios from 'axios'

import { useAppDispatch, RootState } from '../../reducers/store'
import { LoginOnEndpoint, RegisterOnEndpoint, setAuthentication, removeAuthentication } from './authSlice'

import { useMicroService as useMicroServiceRouter } from '../microservice/useMicroServiceRouter'

export const useJWTAuth = () => {

    const microServiceRouter = useMicroServiceRouter();
    const dispatch = useAppDispatch();
    const { user, authenticated } = useSelector( (state: RootState) => state.auth )

    const Login = (email: string, password: string) => {
        
      microServiceRouter.IterateMicroServiceEndpoints('auth',
        async (endpoint) => {
          const status = await dispatch(LoginOnEndpoint(endpoint, email, password));
          return  status === 401 && status === 200;
        }
      );
      
    };

    const Logout = () => {
      window.localStorage.clear();
      delete axios.defaults.headers.common['Authorization'];
      dispatch(removeAuthentication(user));
    };

    const Register = (email: string, password: string, firstName: string, lastName: string) => {
  
      microServiceRouter.IterateMicroServiceEndpoints('auth',
        async (endpoint) => {
          const status = await dispatch(RegisterOnEndpoint(endpoint, email, password, firstName, lastName));
          return  status === 401 && status === 200;
        }
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