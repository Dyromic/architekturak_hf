//import { useEffect } from "react";
import { useSelector } from 'react-redux'
import jwt_decode from 'jwt-decode'

import axios from 'axios'

import { useAppDispatch, RootState } from '../../reducers/store'
import { signin, signout } from './authSlice'
import { User } from './User'

export const useJWTAuth = () => {

    const dispatch = useAppDispatch();
    const { user, authenticated } = useSelector( (state: RootState) => state.auth )

    const login = (email: string, password: string) => {

      return axios.post('/login', {
        email: email,
        password: password
      }).then((response) => {
        if (response.status === 200) {
          const user = jwt_decode<User>(response.data);
          axios.defaults.headers.common['Authorization'] = `Bearer ${response.data}`;
          dispatch(signin(user));
          return true;
        }
        return false;
      })
      .catch((error) => {
        return false;
      });

    };

    const logout = () => {
      delete axios.defaults.headers.common['Authorization'];
      dispatch(signout(user));
    };

    const register = () => {
      axios.post('/register', {
        email: 'Fred',
        password: 'Flintstone'
      }).then(function (response) {
        if (response.status === 200) {
          const user = jwt_decode<User>(response.data);
          axios.defaults.headers.common['Authorization'] = `Bearer ${response.data}`;
          dispatch(signin(user));
        }
      })
    };

    const isAuthenticated = () => {
        return authenticated;
    }

    return {
        user,
        isAuthenticated,
        login,
        logout,
        register
    };

};