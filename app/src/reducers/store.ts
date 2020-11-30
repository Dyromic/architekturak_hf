import { configureStore } from '@reduxjs/toolkit';
import { combineReducers } from 'redux';
import { useDispatch } from 'react-redux';
//import todosReducer from 'features/todos/todosSlice'
//import visibilityFilterReducer from 'features/filters/filtersSlice'

import authReducer from './../features/auth/authSlice';
import microServiceReducer from './../features/microservice/microServiceSlice';
import configurationReducer from '../features/configurator/configurationSlice';

const rootReducer = combineReducers({
  auth: authReducer,
  microservice: microServiceReducer,
  config: configurationReducer
});

export type RootState = ReturnType<typeof rootReducer>

const store = configureStore({
    reducer: rootReducer
  });

export default store;

export type AppDispatch = typeof store.dispatch
export const useAppDispatch = () => useDispatch<AppDispatch>() 