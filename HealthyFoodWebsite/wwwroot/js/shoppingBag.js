
"use strict";

// Deletion function of the shopping bag items

function cleanShoppingBagAfterDeletingAllItems() {

    const pageFirstMainELement = document.getElementById("page-main-element-1");
    const pageSecondMainELement = document.getElementById("page-main-element-2");
    const pageThirdMainELement = document.getElementById("page-main-element-3");
    const pageFourthMainELement = document.getElementById("page-main-element-4");
    const pageFifthMainELement = document.getElementById("order-confirmation");

    pageFirstMainELement.classList.add(["d-none"]);
    pageSecondMainELement.classList.remove(["d-none"]);
    pageThirdMainELement.classList.add(["d-none"]);
    pageFourthMainELement.classList.add(["d-none"]);
    pageFifthMainELement.classList.add(["d-none"]);
}
async function deleteItemAsync(id) {
    $.support.cors = true;
    await $.ajax({
        url: `/ShoppingBag/Delete/${id}`,
        type: "GET",
        cache: false,
        success: (result, status, xhr) => {
            if (result == true) {
                const item = document.getElementById(`item-number-${id}`);
                item.remove();

                const itemsTableBody = document.getElementById("items-table-body");

                if (itemsTableBody != null) {

                    const items = itemsTableBody.children;

                    if (items.length > 0) {

                        let totalPrice = 0;

                        Array.from(items).forEach(item => {
                            const quantity = Number(item.children[4].children[0].value);
                            const unitPrice = Number(item.children[5].textContent);
                            totalPrice += (quantity * unitPrice);
                        });

                        const totalPriceElement = document.getElementById("total-price");
                        totalPriceElement.textContent = totalPrice;
                    }
                    else {
                        cleanShoppingBagAfterDeletingAllItems();
                    }

                }
            }
        },
        error: (xhr, status, error) => { }
    });
}


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


async function relateShoppingBagItemsWithCurrentOrder(currentOrderId) {

    // Relating The Confirmed Shopping Bag Items With The Current Constructed Order.

    const shoppingBagItems = document.getElementsByClassName('shopping-bag-items');

    shoppingBag = [];

    Array.from(shoppingBagItems).forEach(item => {
        shoppingBag.push({
            id: Number(item.children[1].textContent),
            name: item.children[2].textContent,
            unitPrice: Number(item.children[3].textContent),
            quantity: Number(item.children[4].firstElementChild.value),
            subTotalPrice: Number(item.children[5].textContent),
            status: "Confirmed",
            orderId: currentOrderId,
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
        success: (result, status, xhr) => { },
        error: (xhr, status, error) => { }
    });
}
function redirectCurrentOrderFromCustomerToSellerThenCleanBag(currentOrderId) {

    // Redirecting The Current Order From Customer To The Seller (Using SignalR).

    connection.invoke("RedirectOrderFromCustomerToSeller", currentOrderId)
    .catch(function (err) {
        return console.error(err.toString());
    });

    // Cleaning The Shopping Bag Environment After Confirming The Current Order
    cleanShoppingBagAfterDeletingAllItems();
}


connection.on("SendOrderIdAsync", async (currentOrderId) => {

    await relateShoppingBagItemsWithCurrentOrder(currentOrderId);

    await redirectCurrentOrderFromCustomerToSellerThenCleanBag(currentOrderId);

});

connection.on("SendOrderToUserAsync", (currentOrder) => {

    console.log(currentOrder);

    const userCardsContainer = document.getElementById("userCardsContainer");

    const cardWrapper = document.createElement("div");
    cardWrapper.classList.add(["card"]);
    cardWrapper.classList.add(["text-start"]);
    cardWrapper.classList.add(["mb-3"]);
    cardWrapper.style.width = "40%";

    const cardBody = document.createElement("div");
    cardBody.classList.add(["card-body"]);


    const currentOrderNumber = document.getElementsByClassName("ordersCount").length + 1;

    var orderDateAndTime = currentOrder.initiatingDateAndTime.split('T');

    cardBody.innerHTML = `
    <h5 class="card-title">Order <span class="text-muted ordersCount">#${currentOrderNumber}</span></h5>

    <table class="table table-hover align-middle">
        <tbody>
            <tr>
                <td class="fw-bold">Total Cost (EGP)</td>
                <td>${currentOrder.totalCost}</td>
            </tr>
            <tr>
                <td class="fw-bold">Date-Time</td>
                <td>${orderDateAndTime[0]} ${orderDateAndTime[1]}</td>
            </tr>
            <tr>
                <td class="fw-bold">Order Status</td>
                <td>${currentOrder.status}</td>
            </tr>
        </tbody>
    </table>

    <div class="d-flex justify-content-center">
        <div class="fw-bold text-decoration-underline">Order Details</div>
    </div>

    <table class="table table-hover align-middle">
        <thead>
            <tr>
                <th scope="col">Product Name</th>
                <th scope="col">Unit Price (EGP)</th>
                <th scope="col">Quantity (Kg)</th>
                <th scope="col">Sub-Total (EGP)</th>
            </tr>
        </thead>
        <tbody id="productsTableBody-${currentOrder.orderId}"></tbody>
    </table>
`

    cardWrapper.appendChild(cardBody);
    userCardsContainer.appendChild(cardWrapper);


    const productsTableBody = document.getElementById(`productsTableBody-${currentOrder.orderId}`);

    currentOrder.shoppingBagItems.forEach(item => {

        const tr = document.createElement('tr');

        const td1 = document.createElement('td');
        td1.textContent = `${item.productName}`;

        const td2 = document.createElement('td');
        td2.textContent = `${item.price}`;

        const td3 = document.createElement('td');
        td3.textContent = `${item.quantity}`;

        const td4 = document.createElement('td');
        td4.textContent = `${item.subTotal}`;

        tr.appendChild(td1);
        tr.appendChild(td2);
        tr.appendChild(td3);
        tr.appendChild(td4);

        productsTableBody.appendChild(tr);
    });

});



async function constructThenRelateThenRedirect(event)
{
    // Constructing A New Order To Relate It With The Confirmed Shopping Bag Items.

    let orderDetails = JSON.stringify({
        id: 0,
        status: "Active",
        initiatingDateAndTime: (new Date()).toLocaleString(),
        startedPreparing: false,
        startedDelivering: false,
        totalCost: Number(totalPriceElement.textContent.split(' ')[1]),
        loggerId: 1 //TODO: Get the correct Logger Id here.
    });

    connection.invoke("PersistOrderInDatabaseThenReturnId", orderDetails)
        .catch(function (err) {
            return console.error(err.toString());
        });

    event.preventDefault();
}
