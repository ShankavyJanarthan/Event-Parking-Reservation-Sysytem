async function loadBookings() {


    let data =
        await apiCall(
            "/bookings/my"
        );



    let html = "";



    data.forEach(b => {


        html += `


<div class="card">


<h3>
Booking ID : ${b.bookingId}
</h3>


<p>
Status : ${b.status}
</p>


<button
class="btn"
onclick="cancelBooking(${b.bookingId})">

Cancel

</button>


</div>


`;


    });



    document.getElementById("bookings")
        .innerHTML =
        html;


}




async function cancelBooking(id) {


    await apiCall(

        "/bookings/" + id + "/cancel",

        "POST"

    );


    alert(
        "Booking Cancelled"
    );


    location.reload();


}



loadBookings();