async function loadProfile() {


    let id =
        localStorage.getItem("customerId");



    let user =
        await apiCall(
            "/customers/" + id
        );



    document.getElementById("name").value =
        user.fullName;


    document.getElementById("email").value =
        user.email;


    document.getElementById("phone").value =
        user.phoneNumber;


}



async function updateProfile() {


    let id =
        localStorage.getItem("customerId");



    let data = {


        fullName:
            document.getElementById("name").value,


        email:
            document.getElementById("email").value,


        phoneNumber:
            document.getElementById("phone").value


    };



    await apiCall(

        "/customers/" + id,

        "PUT",

        data

    );



    document.getElementById("msg")
        .innerHTML =
        "Profile Updated Successfully";


}



loadProfile();