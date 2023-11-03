
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
                            const unitPrice = Number(item.children[3].textContent);
                            const quantity = Number(item.children[4].firstElementChild.value);
                            totalPrice += (unitPrice * quantity);
                        });

                        const totalPriceElement = document.getElementById("total-price");
                        totalPriceElement.textContent = `EGP ${totalPrice}`;
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

function appendNewConstructedOrderInActiveOrdersGridToBeTracked(currentOrderId) {

    const newTrackedOrderNumber = document.getElementsByClassName('tracked-active-orders').length + 1;

    document.getElementById('tracked-orders-container').insertAdjacentHTML("beforeend",
        `
            <div id="tracked-user-active-order-${currentOrderId}" class="tracked-active-orders d-flex justify-content-center align-items-center shadow bg-body-tertiary py-3 mb-4">

                <span>#${newTrackedOrderNumber} |</span>
                                
                <span class="fw-bold ms-2 me-3">Order-ID <span>${currentOrderId}</span></span>

                <div class="progress d-inline me-1" style="width: 40%; height: 25px;" role="progressbar" aria-label="Animated striped progress bar" aria-valuenow="75" aria-valuemin="0" aria-valuemax="100">
                    <div class="progress-bar progress-bar-striped progress-bar-animated fw-bold" style="width: 0%; height: 25px;"></div>
                </div>

                <div class="progress d-inline ms-2 me-1" style="width: 40%; height: 25px;" role="progressbar" aria-label="Animated striped progress bar" aria-valuenow="75" aria-valuemin="0" aria-valuemax="100">
                    <div class="progress-bar progress-bar-striped progress-bar-animated bg-info fw-bold" style="width: 0%; height: 25px;"></div>
                </div>

            </div>
        `
    );
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

    appendNewConstructedOrderInActiveOrdersGridToBeTracked(currentOrderId);

    await redirectCurrentOrderFromCustomerToSellerThenCleanBag(currentOrderId);

});

connection.on("SendOrderToUserAsync", (currentOrder) => {

    console.log(currentOrder);

    const userCardsContainer = document.getElementById("userCardsContainer");

    const cardWrapper = document.createElement("div");
    cardWrapper.id = `user-confirmed-order-${currentOrder.orderId}`;
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
                <td class="fw-bold">Order ID</td>
                <td>${currentOrder.orderId}</td>
            </tr>
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

    <div class="d-flex flex-column justify-content-center">
        <!-- Deletion button trigger modal -->
        <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#confirmedOrderDeletionModal-${currentOrder.orderId}">Delete</button>
    </div>
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

    const myTabContent = document.getElementById('myTabContent');

    myTabContent.insertAdjacentHTML('beforeend',
        `
            <!-- Deletion modal -->
            <div class="modal fade" id="confirmedOrderDeletionModal-${currentOrder.orderId}" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="deletionStaticBackdrop" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h1 class="modal-title fs-5" id="staticBackdropLabel">Deletion Confirmation!</h1>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this confirmed order?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                            <button type="button" class="btn btn-primary" data-bs-dismiss="modal" onclick="performUserViewDeletionAsync(${currentOrder.orderId})">Understood</button>
                        </div>
                    </div>
                </div>
            </div>
        `
    );

});


async function constructThenRelateThenRedirect(event)
{
    // Constructing A New Order To Relate It With The Confirmed Shopping Bag Items.

    let currentLoggerId = Number(document.getElementById('current-logger-id').textContent.trim());

    let orderDetails = JSON.stringify({
        id: 0,
        status: "Active",
        initiatingDateAndTime: (new Date()).toLocaleString(),
        startedPreparing: false,
        startedDelivering: false,
        totalCost: Number(totalPriceElement.textContent.split(' ')[1]),
        loggerId: currentLoggerId
    });

    connection.invoke("PersistOrderInDatabaseThenReturnId", orderDetails)
        .catch(function (err) {
            return console.error(err.toString());
        });

    event.preventDefault();
}



// The section of order inactive status and mode

connection.on("SendOrderIdWithItsInactiveStatusToUserAsync", (id, orderStatus) => {

    const targetTrackedActiveOrder = document.getElementById(`tracked-user-active-order-${id}`);
    targetTrackedActiveOrder.remove();

    const targetOrderCard = document.getElementById(`user-confirmed-order-${id}`);

    targetOrderCard.firstElementChild.children[1].firstElementChild.children[3].lastElementChild.textContent = `${orderStatus}`;

});

connection.on("SendOrderIdWithItsModeToUserAsync", (id, mode) => {

    const targetTrackedActiveOrderChildren = document.getElementById(`tracked-user-active-order-${id}`).children;

    const preparingProgressBar = targetTrackedActiveOrderChildren[2].firstElementChild;

    const deliveringProgressBar = targetTrackedActiveOrderChildren[3].firstElementChild;

    if (mode == "Preparing") {
        preparingProgressBar.style.width = "50%";
        preparingProgressBar.textContent = `Now Preparing...`;
    }
    else if (mode == "Delivering") {
        preparingProgressBar.style.width = "100%";
        preparingProgressBar.textContent = `Order Prepared!`;

        deliveringProgressBar.style.width = "50%";
        deliveringProgressBar.textContent = `Now Delivering...`;
    }
});



// The section of writing "StartedPreparing" and "StartedDelivering" properties values from database to the shopping bag active orders grid

$(document).ready(() => {

    const trackedActiveOrders = document.getElementsByClassName('tracked-active-orders');

    Array.from(trackedActiveOrders).forEach(order => {

        const preparingValue = order.lastElementChild.firstElementChild.textContent;
        const deliveringValue = order.lastElementChild.lastElementChild.textContent;

        const preparingProgressBar = order.children[2].firstElementChild;
        const deliveringProgressBar = order.children[3].firstElementChild;

        if (preparingValue == "True" && deliveringValue == "False") {
            preparingProgressBar.textContent = "Now Preparing..."
            preparingProgressBar.style.width = "50%";
        }
        else if (deliveringValue == "True") {
            preparingProgressBar.textContent = "Order Prepared!"
            preparingProgressBar.style.width = "100%";

            deliveringProgressBar.textContent = "Now Delivering..."
            deliveringProgressBar.style.width = "50%";
        }
    });
});



// Deleting an order from an admin view perspective
async function performUserViewDeletionAsync(orderId) {
    $.support.cors = true;
    await $.ajax({
        url: `/Order/PerformUserOrAdminViewDeletion/${orderId}?view=UserView`,
        type: "GET",
        cache: false,
        success: (result, status, xhr) => {
            if (result == true) {

                connection.invoke("RedirectSpecificOrderIdFromUserToSeller", orderId)
                .catch(function (err) {
                    return console.error(err.toString());
                });

                const targetConfirmedOrderCard = document.getElementById(`user-confirmed-order-${orderId}`);

                targetConfirmedOrderCard.remove();
            }
        },
        error: (xhr, status, error) => { }
    });
}