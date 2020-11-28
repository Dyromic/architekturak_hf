import { createSlice, PayloadAction } from '@reduxjs/toolkit'

export type MicroServiceName = string;
export type Endpoints = string[];
export type MicroServiceEndpoints = {
    name: MicroServiceName,
    endpoints: Endpoints
};
export type MicroServiceEndpointDictionary = { [name: string]: Endpoints };
export type MicroServiceState = { 
    services?: MicroServiceEndpointDictionary 
};

const initialMicroServiceState: MicroServiceState = {
    services: undefined
};

const microServiceSlice = createSlice({
  name: 'microservice',
  initialState: initialMicroServiceState,
  reducers: {
    addServiceEndpoints: (state, action: PayloadAction<MicroServiceEndpoints>) => {
        const serviceEndpoint = action.payload;
        if (state.services === undefined) {
            const newServices: MicroServiceEndpointDictionary = {
                [serviceEndpoint.name]: [...serviceEndpoint.endpoints]
            };
            state.services = newServices;
        } else {
            state.services[serviceEndpoint.name] = [...state[serviceEndpoint.name], ...serviceEndpoint.endpoints];
        }
    },
    setServiceEndpoints: (state, action: PayloadAction<MicroServiceEndpoints>) => {
        const serviceEndpoint = action.payload;
        if (state.services === undefined) {
            const newServices: MicroServiceEndpointDictionary = {
                [serviceEndpoint.name]: [...serviceEndpoint.endpoints]
            };
            state.services = newServices;
        } else {
            state.services[serviceEndpoint.name] = [...serviceEndpoint.endpoints];
        }
    },
    removeServiceEndpoints: (state, action: PayloadAction<MicroServiceEndpoints>) => {
        const serviceEndpoint = action.payload;
        if (state.services !== undefined && serviceEndpoint.name in state.services) {
            delete state.services[serviceEndpoint.name];
        }
    }

  }
})

export const { 
    addServiceEndpoints, 
    setServiceEndpoints, 
    removeServiceEndpoints 
} = microServiceSlice.actions;

export default microServiceSlice.reducer;
