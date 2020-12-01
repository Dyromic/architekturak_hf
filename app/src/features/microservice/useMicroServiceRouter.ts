import { useSelector } from 'react-redux'

import axios from 'axios'

import { RootState, AppDispatch } from '../../reducers/store'
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

        if (!serviceAvailable) {

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

    const getSingleServiceEndpoint = (name: MicroServiceName): (string | null) => {
        const items = services[name];
        if (!serviceAvailable || !items || items.length <= 0) return null;
        return items[Math.floor(Math.random() * items.length)];
    };

    const validStatus = (status: number): boolean => {
        return status === 400 || status === 401 || status === 200;
    };

    const post = async (name: MicroServiceName, route: string, data?: any) => {

        if (!serviceAvailable) return;
        for (let endpoint of services[name]) {
            try {
                const response = await axios.post(`${endpoint}/${route}`, data);
                if (response !== undefined && validStatus(response.status)) {
                    return response;
                }
            } catch (err) {

            } 
        }

    };

    const put = async (name: MicroServiceName, route: string, data: any) => {

        if (!serviceAvailable) return;
        for (let endpoint of services[name]) {
            try {
                const response = await axios.put(`${endpoint}/${route}`, data);
                if (response !== undefined && validStatus(response.status)) {
                    return response;
                }
            } catch (err) {

            } 
        }

    };

    const get = async (name: MicroServiceName, route: string, options: any = undefined) => {

        if (!serviceAvailable) return;
        for (let endpoint of services[name]) {
            try {
                const response = await axios.get(`${endpoint}/${route}`, options);
                if (response !== undefined && validStatus(response.status)) {
                    return response;
                }
            } catch (err) {

            } 
        }

    };

    return {
        getSingleServiceEndpoint,
        requestServiceEndpoints,
        serviceAvailable,
        post,
        get,
        put
    };

};
