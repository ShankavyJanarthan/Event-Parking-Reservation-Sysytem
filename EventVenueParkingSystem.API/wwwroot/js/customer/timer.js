let time = 15 * 60;


let timer =
    document.getElementById("timer");



let countdown =
    setInterval(function () {


        let minutes =
            Math.floor(time / 60);



        let seconds =
            time % 60;



        seconds =
            seconds < 10 ?
                "0" + seconds :
                seconds;



        timer.innerHTML =
            minutes + ":" + seconds;



        time--;



        if (time < 0) {


            clearInterval(countdown);



            document.getElementById("message")
                .innerHTML =
                "Booking Expired";



            timer.innerHTML =
                "00:00";


        }


    }, 1000);





// Load selected seat

let seat =
    localStorage.getItem("selectedSeat");


document.getElementById("seat")
    .innerHTML =
    seat || "Not Selected";





function goPayment() {


    window.location.href =
        "payment.html";


}