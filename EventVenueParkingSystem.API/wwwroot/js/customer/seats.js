let selectedSeat = null;



async function loadSeats() {


    let eventId =
        document.getElementById("eventId").value;



    let seats =
        await apiCall(
            "/seats/event/" + eventId,
            "GET"
        );



    let html = "";



    seats.forEach(seat => {


        let status =
            seat.isBooked ?
                "booked" :
                "";



        html += `


<div

class="seat ${status}"

onclick="selectSeat(${seat.seatId},this)"

>

${seat.seatNumber}


</div>


`;


    });



    document.getElementById("seatArea")
        .innerHTML =
        html;


}




function selectSeat(id, element) {


    if (element.classList.contains("booked")) {

        return;

    }



    document
        .querySelectorAll(".seat")
        .forEach(s =>
            s.classList.remove("selected")
        );



    element.classList.add("selected");


    selectedSeat = id;



    document.getElementById("msg")
        .innerHTML =
        "Selected Seat ID : " + id;


}




function continueBooking() {


    if (selectedSeat == null) {

        alert(
            "Please select a seat"
        );

        return;

    }



    localStorage.setItem(
        "selectedSeat",
        selectedSeat
    );



    window.location.href =
        "booking.html";


}