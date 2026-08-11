async function loadNotifications() {


    let data =
        await apiCall(
            "/notifications/my"
        );



    let html = "";



    data.forEach(n => {


        html += `

<div class="card">


<p>
${n.message}
</p>


<button 
class="btn"
onclick="readNotification(${n.notificationId})">

Mark Read

</button>


</div>


`;



    });



    document.getElementById("notifications")
        .innerHTML =
        html;


}



async function readNotification(id) {


    await apiCall(

        "/notifications/" + id + "/read",

        "POST"

    );



    alert(
        "Notification Read"
    );


    loadNotifications();


}



loadNotifications();