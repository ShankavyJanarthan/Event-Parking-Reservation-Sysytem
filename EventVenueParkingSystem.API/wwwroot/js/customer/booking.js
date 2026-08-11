async function book() {


    let data = {

        eventId:
            Number(eventId.value),

        seatId:
            Number(seatId.value),

        parkingSlotId:
            parkingId.value ?
                Number(parkingId.value) : null

    };


    try {


        let result =
            await apiCall(
                "/bookings",
                "POST",
                data
            );


        msg.innerHTML =
            "Booking Created";


    }

    catch (e) {

        msg.innerHTML = e.message;

    }


}