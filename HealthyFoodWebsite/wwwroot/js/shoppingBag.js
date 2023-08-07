
// The Sub-Total Prices And Total Price Calculations Section

const totalPriceElement = document.getElementById('total-price');

function calculateTotalPrice(price, quantity, itemId)
{
    const elementId = `sub-total-price-${itemId}`;
    const subTotalPriceElement = document.getElementById(elementId);
    const orderItems = document.getElementsByClassName('sub-totals-sum');
    let totalPrice = 0;

    subTotalPriceElement.textContent = price * Number(quantity);

    Array.from(orderItems).forEach(htmlElement => {
        totalPrice += Number(htmlElement.textContent);
    });

    totalPriceElement.textContent = `EGP ${totalPrice}`;
}


// The Section Of Order SignalRing And Persisting Current Order Details In The Database

let shoppingBag = [];

var connection = new signalR.HubConnectionBuilder().withUrl("/order-hub").build();
document.getElementById("order-confirmation").disabled = true;

connection.start().then(function () {
    document.getElementById("order-confirmation").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


async function persistCurrentOrderInDatabase(event)
{
    "use strict";

    // Putting Current Order In The Database

    const shoppingBagItems = document.getElementsByClassName('shopping-bag-items');

    shoppingBag = [];

    Array.from(shoppingBagItems).forEach(item => {
        shoppingBag.push({
            id: Number(item.children[1].textContent),
            name: item.children[2].textContent,
            price: Number(item.children[3].textContent),
            quantity: Number(item.children[4].firstElementChild.value),
            subTotalPrice: Number(item.children[5].textContent),
            status: "Confirmed",
            loggerId: Number(item.children[6].textContent)
        });
    });

    let shoppingBagStringified = JSON.stringify(shoppingBag);

    $.support.cors = true;
    await $.ajax({
        url: "/ShoppingBag/UpdateUsingJsonObjectsArray",
        type: "POST",
        data: { itemsArray: shoppingBagStringified },
        dataType: "JSON",
        cache: false,
        success: (result, status, xhr) => {  },
        error: (xhr, status, error) => {  }
    });


    // SignalRing Section

    let orderDetails = JSON.stringify({
        id: 0,
        status: "Active",
        initiatingDateAndTime: (new Date()).toLocaleString(),
        startedPreparing: false,
        startedDelivering: false,
        loggerId: shoppingBag[0].loggerId
    });

    connection.invoke("PersistOrderInDatabaseThenRedirect", orderDetails).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}
