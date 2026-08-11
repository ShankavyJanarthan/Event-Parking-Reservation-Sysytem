// ================================
// REGISTER
// ================================

document
    .getElementById("registerForm")
    ?.addEventListener(
        "submit",
        async function (e) {

            e.preventDefault();


            let customer = {

                fullName:
                    document.getElementById("fullName").value,


                email:
                    document.getElementById("email").value,


                phoneNumber:
                    document.getElementById("phoneNumber").value,


                password:
                    document.getElementById("password").value

            };


            try {

                await apiCall(
                    "/customers/register",
                    "POST",
                    customer
                );


                document.getElementById("message")
                    .innerHTML =
                    "Registration Successful";


                setTimeout(() => {

                    window.location.href =
                        "login.html";

                }, 1000);


            }

            catch (error) {

                document.getElementById("message")
                    .innerHTML =
                    error.message;

            }


        });




// ================================
// LOGIN
// ================================

document
    .getElementById("loginForm")
    ?.addEventListener(
        "submit",
        async function (e) {


            e.preventDefault();



            let loginData = {


                email:
                    document.getElementById("loginEmail").value,


                password:
                    document.getElementById("loginPassword").value


            };



            try {


                let result =
                    await apiCall(
                        "/auth/login",
                        "POST",
                        loginData
                    );



                // Save JWT Token

                localStorage.setItem(
                    CONFIG.TOKEN,
                    result.token
                );



                localStorage.setItem(
                    "user",
                    JSON.stringify(result)
                );



                document.getElementById("loginMessage")
                    .innerHTML =
                    "Login Successful";



                // Correct dashboard path

                setTimeout(() => {


                    window.location.href =
                        "../customer/dashboard.html";


                }, 1000);



            }

            catch (error) {


                document.getElementById("loginMessage")
                    .innerHTML =
                    "Invalid Email or Password";


            }


        });