import { useEffect, useSelector } from 'react-redux'

import axios from 'axios'

import { useAppDispatch, RootState } from '../../reducers/store'
import {ContentProviderAPI} from '../../config/config'

import {   
    Endpoints,
    addServiceEndpoints, 
    setServiceEndpoints, 
    removeServiceEndpoints,  
    MicroServiceEndpoints,
    MicroServiceName
} from './microServiceSlice'

export const useMicroService = () => {

    const dispatch = useAppDispatch();
    const { microServices } = useSelector( (state: RootState) => state.microservice );

    const requestServiceEndpoints = async () => {

        if (microServices === undefined) {

            const response = await axios.get(`${ContentProviderAPI}/routes`);
            if (response === undefined || response.status !== 200) return false;     

            const serviceRoutes = response.data as MicroServiceEndpoints[];
            serviceRoutes.forEach((item) => {
                const newMicroService: MicroServiceEndpoints = {
                    name: item.name,
                    endpoints: item.endpoints
                };

                dispatch(addServiceEndpoints(newMicroService));

            });            

        }

    };

    const getServiceEndpoints = (name: MicroServiceName): (Endpoints | null) => {

        if (microServices === undefined) return null;
        return microServices[name];
        
    };

    const IterateServiceEndpoints = (name: MicroServiceName, cb: (endpoint:string) => Promise<boolean>) => {

        if (microServices === undefined) return;
        (async () => {
            for (let endpoint of microServices[name]) {
                const val = await cb(endpoint);
                if (val) break;
            }
        })();
        
    };

    useEffect(() => {

        requestServiceEndpoints();

    }, []);

    return {
        requestServiceEndpoints,
        getServiceEndpoints,
        IterateMicroServiceEndpoints: IterateServiceEndpoints
    };

};
