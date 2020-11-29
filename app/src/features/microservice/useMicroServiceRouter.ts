import { useSelector } from 'react-redux'

import axios from 'axios'

import { useAppDispatch, RootState } from '../../reducers/store'
import {ContentProviderAPI} from '../../config/config'

import {   
    setAvailable,
    addServiceEndpoints, 
    MicroServiceEndpoints,
    MicroServiceName
} from './microServiceSlice'

export const useMicroService = () => {

    const dispatch = useAppDispatch();
    //const history = useHistory();
    const { microServices, serviceAvailable } = useSelector( (state: RootState) => state.microservice );

    const requestServiceEndpoints = () => async () => {

        if (microServices === undefined) {

            try {

                const response = await axios.get(`${ContentProviderAPI}/routes`);

                const serviceRoutes = response.data as MicroServiceEndpoints[];
                serviceRoutes.forEach((item) => {
                    const newMicroService: MicroServiceEndpoints = {
                        name: item.name,
                        endpoints: item.endpoints
                    };
    
                    dispatch(addServiceEndpoints(newMicroService));
    
                });            
                dispatch(setAvailable(true));

            } catch (err) {
                console.log(err);
                dispatch(setAvailable(false));
            }

        }

    };

    const validStatus = (status: number): boolean => {
        return status === 401 || status === 200;
    }

    const post = async (name: MicroServiceName, route: string, data: any) => {

        if (!serviceAvailable) return;
        for (let endpoint of microServices[name]) {
            const response = await axios.post(`${endpoint}/${route}`, data);
            if (response !== undefined && validStatus(response.status)) {
                return response;
            }
        }

    };

    const get = async (name: MicroServiceName, route: string, options: any) => {

        if (!serviceAvailable) return;
        for (let endpoint of microServices[name]) {
            const response = await axios.get(`${endpoint}/${route}`, options);
            if (response !== undefined && validStatus(response.status)) {
                return response;
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
