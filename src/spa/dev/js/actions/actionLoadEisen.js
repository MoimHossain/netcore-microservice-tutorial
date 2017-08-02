var ___x = [
        {
            id: '1',
            title: 'Hyper-loop',
            modelImage: 'http://www.stonybrook.edu/happenings/wp-content/uploads/Hyperloop-interior.jpg',
            description: "Build hyper-loop tunnel from Amsterdam to Madrid",
            phase: "Initial ontwerp"
        }
    ];

function getAllEisenAsync(data) {
    return {
        type: "GET_EISEN",
        payload: data
    };
}


export default function getAllEisen() {
    return dispatch => {        
        axios.get('/api/eisen')
             .then(res => {
                 console.log('WE HAVE DATA!!');
                 console.log(res);
                 dispatch(getAllEisenAsync(res.data));
             }, error => {
                 console.log(error);
                 dispatch(getAllEisenAsync(___x));
             });
    }
}