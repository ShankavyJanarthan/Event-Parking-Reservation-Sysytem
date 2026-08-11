async function pay() {


    let card =
        document.getElementById("card").value;


    let cvv =
        document.getElementById("cvv").value;



    if (card.length < 12) {

        msg.innerHTML =
            "Enter valid card number";

        return;

    }



    if (cvv.length < 3) {

        msg.innerHTML =
            "Invalid CVV";

        return;

    }



    document.getElementById("loader")
        .innerHTML =
        "Processing Payment...";



    let bookingId =
        localStorage.getItem("bookingId");



    try {


        await apiCall(

            "/payments/booking/" + bookingId,

            "POST",

            {

                paymentMethod:
                    document.getElementById("method").value

            }

        );



        msg.innerHTML =
            "✅ Payment Successful";


        setTimeout(() => {


            window.location.href =
                "bookings.html";


        }, 1500);



    }

    catch (e) {


        msg.innerHTML =
            "Payment Failed";


    }


}