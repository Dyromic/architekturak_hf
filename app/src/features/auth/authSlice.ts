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

    signin: (state, action: PayloadAction<User>) => {
        const user = action.payload;
        state.authenticated = true;
        state.user = user;
    },
    signout: (state, action: PayloadAction<User>) => {
        if (state.authenticated === true) {
            state.authenticated = false;
            state.user = undefined;
        }
    }

    /*addTodo: {
      reducer(state, action: PayloadAction<User[]>) {
        const { id, text } = action.payload
        state.push({ id, text, completed: false })
      },
      prepare(text) {
        return { payload: { text, id: nextTodoId++ } }
      }
    },
    toggleTodo(state, action: PayloadAction<User[]>) {
      /*const todo = state.find(todo => todo.id === action.payload)
      if (todo) {
        todo.completed = !todo.completed
      }
    }*/
  }
})

export const { signin, signout } = authSlice.actions

export default authSlice.reducer
