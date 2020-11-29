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

        if (!serviceAvailable) {

            try {

                const response = await axios.get(`${ContentProviderAPI}`);

                console.log(response);

                if (response.status === 200) {
                    console.log("Siker!");
                    const serviceRoutes = response.data as MicroServiceEndpoints[];
                    serviceRoutes.forEach((item) => {
                        const newMicroService: MicroServiceEndpoints = {
                            name: item.name,
                            endpoints: item.endpoints
                        };
                        console.log(item);
                        dispatch(addServiceEndpoints(newMicroService));
                        console.log("Utana");
        
                    });            
                    dispatch(setAvailable(true));
                } 

            } catch (err) {
                console.log(err)
            }

        }

    };

    const validStatus = (status: number): boolean => {
        return status === 400 || status === 401 || status === 200;
    }

    const post = async (name: MicroServiceName, route: string, data: any) => {

        console.log(`Microservice: ${name}/${route}`);
        console.log(`   - Available: ${serviceAvailable}`)
        if (!serviceAvailable) return;
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

    const get = async (name: MicroServiceName, route: string, options: any) => {

        console.log(`Microservice: ${name}/${route}`);
        console.log(`   - Available: ${serviceAvailable}`)
        if (!serviceAvailable) return;
        for (let endpoint of services[name]) {
            try {
                const response = await axios.get(`${endpoint}/${route}`, options);
                console.log(`GET: ${endpoint}/${route}`);
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
