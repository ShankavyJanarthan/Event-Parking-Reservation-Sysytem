async function loadEvents() {


    let search =
        document.getElementById("search")
            .value;



    let venue =
        document.getElementById("venue")
            .value;



    let category =
        document.getElementById("category")
            .value;



    let events =
        await apiCall(
            "/events",
            "GET"
        );



    let filtered =
        events.filter(e => {


            let nameMatch =
                e.eventName
                    .toLowerCase()
                    .includes(
                        search.toLowerCase()
                    );



            let venueMatch =
                venue == "" ||
                e.venueId == venue;



            let categoryMatch =
                category == "" ||
                e.categoryId == category;



            return nameMatch &&
                venueMatch &&
                categoryMatch;



        });



    displayEvents(filtered);



}




function displayEvents(events) {


    let html = "";



    events.forEach(e => {


        html += `


<div class="card">


<h2>
${e.eventName}
</h2>


<p>
Date:
${e.eventDate}
</p>


<p>
Venue:
${e.venueName || ""}
</p>


<p>
Category:
${e.categoryName || ""}
</p>



<a class="btn"
href="seats.html">

Select Seats

</a>


</div>


`;



    });



    document.getElementById("list")
        .innerHTML =
        html;


}




async function loadFilters() {


    let venues =
        await apiCall(
            "/venues"
        );



    let categories =
        await apiCall(
            "/event-categories"
        );



    let venueHTML =
        "<option value=''>All Venues</option>";



    venues.forEach(v => {


        venueHTML +=
            `

<option value="${v.venueId}">
${v.venueName}
</option>

`;

    });


    document.getElementById("venue")
        .innerHTML =
        venueHTML;




    let categoryHTML =
        "<option value=''>All Categories</option>";



    categories.forEach(c => {


        categoryHTML +=
            `

<option value="${c.categoryId}">
${c.categoryName}
</option>

`;

    });



    document.getElementById("category")
        .innerHTML =
        categoryHTML;


}




loadFilters();

loadEvents();