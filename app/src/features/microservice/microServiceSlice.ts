import { createSlice, PayloadAction } from '@reduxjs/toolkit'

export type MicroServiceName = string;
export type Endpoints = string[];
export type MicroServiceEndpoints = {
    name: MicroServiceName,
    endpoints: Endpoints
};
export type MicroServiceEndpointDictionary = { [name: string]: Endpoints };
export type MicroServiceState = { 
    services: MicroServiceEndpointDictionary,
    serviceAvailable: boolean
};

const initialMicroServiceState: MicroServiceState = {
    services: {},
    serviceAvailable: false
};

const microServiceSlice = createSlice({
  name: 'microservice',
  initialState: initialMicroServiceState,
  reducers: {
    addServiceEndpoints: (state, action: PayloadAction<MicroServiceEndpoints>) => {
        const serviceEndpoint = action.payload;
        if (state.services[serviceEndpoint.name] !== undefined ) {
            const endpoints = state.services[serviceEndpoint.name];
            console.log(endpoints)
            state.services[serviceEndpoint.name] = [...endpoints, ...serviceEndpoint.endpoints];
        } else {
            state.services[serviceEndpoint.name] = [...serviceEndpoint.endpoints];
        }
    },
    setServiceEndpoints: (state, action: PayloadAction<MicroServiceEndpoints>) => {
        const serviceEndpoint = action.payload;
       /* if (state.services === undefined) {
            const newServices: MicroServiceEndpointDictionary = {
                [serviceEndpoint.name]: [...serviceEndpoint.endpoints]
            };
            state.services = newServices;
        } else {*/
            state.services[serviceEndpoint.name] = [...serviceEndpoint.endpoints];
        //}
    },
    removeServiceEndpoints: (state, action: PayloadAction<MicroServiceEndpoints>) => {
        const serviceEndpoint = action.payload;
        if (/*state.services !== undefined &&*/ serviceEndpoint.name in state.services) {
            delete state.services[serviceEndpoint.name];
        }
    },
    setAvailable: (state, action: PayloadAction<boolean>) => {
        const available = action.payload;
        state.serviceAvailable = available;
    }

  }
})

export const { 
    addServiceEndpoints, 
    setServiceEndpoints, 
    removeServiceEndpoints,
    setAvailable
} = microServiceSlice.actions;

export default microServiceSlice.reducer;
