import React, { forwardRef, useEffect } from 'react';
import {
    AddBox,
    Check,
    Clear,
    DeleteOutline,
    ChevronRight,
    Edit,
    SaveAlt,
    FilterList,
    FirstPage,
    LastPage,
    ChevronLeft,
    Search,
    ArrowDownward,
    Remove,
    ViewColumn,
    Save,
  } from "@material-ui/icons";
import MaterialTable, { Icons } from "material-table";
import { useConfigurator } from '../features/configurator/useConfigurator';
import { useAppDispatch } from '../reducers/store';
    
const tableIcons: Icons = {
    Add: forwardRef((props, ref) => <AddBox {...props} ref={ref} />),
    Check: forwardRef((props, ref) => <Check {...props} ref={ref} />),
    Clear: forwardRef((props, ref) => <Clear {...props} ref={ref} />),
    Delete: forwardRef((props, ref) => <DeleteOutline {...props} ref={ref} />),
    DetailPanel: forwardRef((props, ref) => <ChevronRight {...props} ref={ref} />),
    Edit: forwardRef((props, ref) => <Edit {...props} ref={ref} />),
    Export: forwardRef((props, ref) => <SaveAlt {...props} ref={ref} />),
    Filter: forwardRef((props, ref) => <FilterList {...props} ref={ref} />),
    FirstPage: forwardRef((props, ref) => <FirstPage {...props} ref={ref} />),
    LastPage: forwardRef((props, ref) => <LastPage {...props} ref={ref} />),
    NextPage: forwardRef((props, ref) => <ChevronRight {...props} ref={ref} />),
    PreviousPage: forwardRef((props, ref) => <ChevronLeft {...props} ref={ref} />),
    ResetSearch: forwardRef((props, ref) => <Clear {...props} ref={ref} />),
    Search: forwardRef((props, ref) => <Search {...props} ref={ref} />),
    SortArrow: forwardRef((props, ref) => <ArrowDownward {...props} ref={ref} />),
    ThirdStateCheck: forwardRef((props, ref) => <Remove {...props} ref={ref} />),
    ViewColumn: forwardRef((props, ref) => <ViewColumn {...props} ref={ref} />)
};

export const ConversionStatus = () => {

    const dispatch = useAppDispatch();
    const configurator = useConfigurator();    

    useEffect(() => {


        const updateTable = () => {
            dispatch(configurator.getFileNames());
        }
        updateTable();
        const handle = setInterval(updateTable, 2000);

        return () => {
            clearInterval(handle);
        };

    }, [dispatch, configurator]);



    return (
        <MaterialTable
            icons={tableIcons}
            columns={[
                { title: 'Filename', field: 'name' },
                { title: 'Status', field: 'status' }
            ]}
            data={configurator.files.map((f) => ({ name: f.name, status: f.status}))} 
            actions={[
              {
                icon: Save,
                tooltip: 'Download file',
                onClick: (event, rowData) => alert("You saved ")
              }
            ]}
            title="Converted files"
        />
    );
};