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

        dispatch(configurator.getFileNames());

        /*const updateTable = () => {
            console.log("Updated")
            dispatch(configurator.updateStatus());
        }
        
        const handle = setInterval(updateTable, 20000);

        return () => {
            clearInterval(handle);
        };*/
    console.log(configurator);


    }, []);

    
    const convertToFormat = (cat) => {

        const data: any[] = [];
        for (let key in cat) {
            const c = cat[key];
            if (/*c.PptFile && c.SvgFile && */c.Status) data.push({ 
                svgname: "",//c.SvgFile.Name, 
                pptname: "",//c.PptFile.Name, 
                status: c.Status
            });
        }
        return data;
    };


    return (
        <MaterialTable
            icons={tableIcons}
            columns={[
                { title: 'SVG Filename', field: 'svgname' },
                { title: 'PPT Filename', field: 'pptname' },
                { title: 'Status', field: 'status' }
            ]}
            data={convertToFormat(configurator.configs)} 
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