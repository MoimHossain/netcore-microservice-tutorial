export const selectEis = (eis) => {
    console.log("You clicked on eis: ", eis);
    return {
        type: 'EIS_SELECTED',
        payload: eis
    }
};
