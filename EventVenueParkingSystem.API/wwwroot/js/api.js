async function apiCall(
    url,
    method = "GET",
    data = null
) {

    let options = {

        method: method,

        headers: {

            "Content-Type":
                "application/json"

        }

    };


    let token =
        localStorage.getItem(CONFIG.TOKEN);



    if (token) {

        options.headers["Authorization"] =
            "Bearer " + token;

    }


    if (data) {

        options.body =
            JSON.stringify(data);

    }


    let response =
        await fetch(
            CONFIG.API_URL + url,
            options
        );


    return await response.json();

}