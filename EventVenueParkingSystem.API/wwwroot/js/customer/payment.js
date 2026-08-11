async function pay() {


    let id =
        Number(
            document.getElementById("bookingId").value
        );



    let data = {

        paymentMethod: "Simulated"

    };



    try {


        let result =
            await apiCall(
                "/payments/booking/" + id,
                "POST",
                data
            );



        document.getElementById("msg")
            .innerHTML =
            "Payment Successful";


    }

    catch (error) {

        document.getElementById("msg")
            .innerHTML =
            "Payment Failed";

    }


}