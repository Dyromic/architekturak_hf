import { useSelector } from 'react-redux'

import axios from 'axios'

import { useAppDispatch, RootState, AppDispatch } from '../../reducers/store'
import {ContentProviderAPI} from '../../config/config'

import {   
    setAvailable,
    addServiceEndpoints, 
    MicroServiceEndpoints,
    MicroServiceName
} from './microServiceSlice'

export const useMicroService = () => {

    const { services, serviceAvailable } = useSelector( (state: RootState) => state.microservice );

    const requestServiceEndpoints = () => async (dispatch: AppDispatch) => {

        console.log("Request endpoints")

        if (!serviceAvailable) {
            console.log("Request endpoints VALID")
        

            try {

                const response = await axios.get(`${ContentProviderAPI}`);
                if (response.status === 200) {
                    const serviceRoutes = response.data as MicroServiceEndpoints[];
                    serviceRoutes.forEach((item) => {
                        const newMicroService: MicroServiceEndpoints = {
                            name: item.name,
                            endpoints: item.endpoints
                        };
                        dispatch(addServiceEndpoints(newMicroService));
        
                    });            
                    dispatch(setAvailable(true));
                } 

            } catch (err) {
            }

        }

    };

    const validStatus = (status: number): boolean => {
        return status === 400 || status === 401 || status === 200;
    }

    const post = async (name: MicroServiceName, route: string, data: any) => {

        if (!serviceAvailable) return;
        console.log("Microservices available!");
        for (let endpoint of services[name]) {
            try {
                console.log(`POST: ${endpoint}/${route}`);
                const response = await axios.post(`${endpoint}/${route}`, data);
                if (response !== undefined && validStatus(response.status)) {
                    return response;
                }
            } catch (err) {

            } 
        }

    };

    const get = async (name: MicroServiceName, route: string, options: any = undefined) => {

        if (!serviceAvailable) return;
        console.log("Microservices available!");
        for (let endpoint of services[name]) {
            try {
                console.log(`GET: ${endpoint}/${route}`);
                const response = await axios.get(`${endpoint}/${route}`, options);
                if (response !== undefined && validStatus(response.status)) {
                    return response;
                }
            } catch (err) {

            } 
        }

    };

    return {
        requestServiceEndpoints,
        serviceAvailable,
        post,
        get
    };

};
