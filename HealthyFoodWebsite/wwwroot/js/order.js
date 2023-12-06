
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/order-hub").build();

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("SendOrderAsync", function (currentOrder) {

    const cardsContainer = document.getElementById("cardsContainer");

    const cardWrapper = document.createElement("div");
    cardWrapper.id = `order-card-${currentOrder.orderId}`
    cardWrapper.classList.add(["card"]);
    cardWrapper.classList.add(["text-start"]);
    cardWrapper.classList.add(["mb-4"]);

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
                    <td class="fw-bold">Customer Name</td>
                    <td>${currentOrder.customerName}</td>
                </tr>
                <tr>
                    <td class="fw-bold">Phone Number</td>
                    <td>${currentOrder.phoneNumber}</td>
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
                <tr>
                    <td>
                        <label class="form-check-label fw-bold">Preparing</label>
                    </td>
                    <td>
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" role="checkbox" id="firstFlexSwitchCheck" ${currentOrder.startedPreparing == true ? "checked disabled" : ""} onclick="changePreparingToTrue(${currentOrder.orderId}, 'Preparing')">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="form-check-label fw-bold">Delivering</label>
                    </td>
                    <td>
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" role="checkbox" id="secondFlexSwitchCheck" ${currentOrder.startedPreparing == true && currentOrder.startedDelivering == false ? "" : "disabled"} ${currentOrder.startedPreparing == true && currentOrder.startedDelivering == true ? "checked" : ""} onclick="changeDeliveringToTrue(${currentOrder.orderId}, 'Delivering')">
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>

        <div class="d-flex justify-content-center">
            <div class="fw-bold text-decoration-underline">Order Details</div>
        </div>

        <div class="table-responsive">
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
        </div>

        <div class="d-flex justify-content-evenly">
            <!-- Done button trigger modal -->
            <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#doneStaticBackdrop-${currentOrder.orderId}">Done</button>

            <!-- Cancelled button trigger modal -->
            <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#cancelledStaticBackdrop-${currentOrder.orderId}">Cancelled</button>
        </div>
    `

    cardWrapper.appendChild(cardBody);
    cardsContainer.appendChild(cardWrapper);


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
            <!-- Done modal -->
            <div class="modal fade" id="doneStaticBackdrop-${currentOrder.orderId}" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="doneStaticBackdrop" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h1 class="modal-title fs-5" id="staticBackdropLabel">Order Status Confirmation!</h1>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to seal this order as a <strong>"Done"</strong> order ?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                            <button type="button" class="btn btn-primary" data-bs-dismiss="modal" onclick="sealOrderAsDoneOrCancelledAsync(${currentOrder.orderId}, 'Done')">Understood</button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Cancelled modal -->
            <div class="modal fade" id="cancelledStaticBackdrop-${currentOrder.orderId}" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="cancelledStaticBackdrop" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h1 class="modal-title fs-5" id="staticBackdropLabel">Order Status Confirmation!</h1>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to seal this order as a <strong>"Cancelled"</strong> order ?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                            <button type="button" class="btn btn-primary" data-bs-dismiss="modal" onclick="sealOrderAsDoneOrCancelledAsync(${currentOrder.orderId}, 'Cancelled')">Understood</button>
                        </div>
                    </div>
                </div>
            </div>
        `
    );
});


// Sealing a given order as "Done" or "Cancelled" and make necessary actions considering this order
async function sealOrderAsDoneOrCancelledAsync(id, orderStatus) {
    $.support.cors = true;
    await $.ajax({
        url: `/Order/SealOrderAsDoneOrCancelled/${id}?status=${orderStatus}`,
        type: "GET",
        cache: false,
        success: (result, status, xhr) => {
            if (result == true) {

                const thisOrderCard = document.getElementById(`order-card-${id}`);

                thisOrderCard.remove();

                // Preparing this currrent order card to be displayed on the inactive orders tab

                const inactiveOrdersContainer = document.getElementById('inactiveOrdersContainer');

                const newInactiveOrderNumber = inactiveOrdersContainer.children.length + 1;

                thisOrderCard.id = `inactive-order-card-${id}`;

                const orderCardBody = thisOrderCard.firstElementChild;

                orderCardBody.firstElementChild.firstElementChild.textContent = `#${newInactiveOrderNumber}`;

                const orderTableRows = orderCardBody.children[1].firstElementChild.children;

                orderTableRows[5].lastElementChild.textContent = `${orderStatus}`;
                orderTableRows[6].classList.add(["d-none"]);
                orderTableRows[7].classList.add(["d-none"]);

                orderCardBody.children[4].classList.add(["d-none"]);

                orderCardBody.insertAdjacentHTML('beforeend',
                    `
                        <div class="d-flex flex-column justify-content-center">
                            <!-- Deletion button trigger modal -->
                            <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#deletionStaticBackdrop-${id}">Delete</button>
                        </div>
                    `
                );

                inactiveOrdersContainer.insertAdjacentElement('beforeend', thisOrderCard);

                const myTabContent = document.getElementById('myTabContent');

                myTabContent.insertAdjacentHTML('beforeend',
                    `
                        <!-- Deletion modal -->
                        <div class="modal fade" id="deletionStaticBackdrop-${id}" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="deletionStaticBackdrop" aria-hidden="true">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h1 class="modal-title fs-5" id="staticBackdropLabel">Deletion Confirmation!</h1>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                        Are you sure you want to delete this order?
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                                        <button type="button" class="btn btn-primary" data-bs-dismiss="modal" onclick="performAdminViewDeletionAsync(${id})">Understood</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `
                );
            }
        },
        error: (xhr, status, error) => { }
    });

    connection.invoke("RedirectOrderIdWithItsInactiveStatus", id, orderStatus)
    .catch(function (err) {
        return console.error(err.toString());
    });
}


// Changing values of "Preparing" and "Delivering" properties of an order, to true
async function changePreparingToTrue(id, mode) {
    $.support.cors = true;
    await $.ajax({
        url: `/Order/ChangePreparingOrDeliveringToTrue/${id}?mode=${mode}`,
        type: "GET",
        cache: false,
        success: (result, status, xhr) => {
            if (result == true) {

                const targetOrderCard = document.getElementById(`order-card-${id}`);

                const orderProductsTable = targetOrderCard.firstElementChild.children[1].firstElementChild.children;

                const preparingSwitch = orderProductsTable[6].lastElementChild.firstElementChild.firstElementChild;
                const deliveringSwitch = orderProductsTable[7].lastElementChild.firstElementChild.firstElementChild;

                preparingSwitch.disabled = true;
                deliveringSwitch.disabled = false;

                connection.invoke("RedirectOrderIdWithItsMode", id, mode)
                .catch(function (err) {
                    return console.error(err.toString());
                });
            }
        },
        error: (xhr, status, error) => { }
    });
}


async function changeDeliveringToTrue(id, mode) {
    $.support.cors = true;
    await $.ajax({
        url: `/Order/ChangePreparingOrDeliveringToTrue/${id}?mode=${mode}`,
        type: "GET",
        cache: false,
        success: (result, status, xhr) => {
            if (result == true) {

                const targetOrderCard = document.getElementById(`order-card-${id}`);

                const orderProductsTable = targetOrderCard.firstElementChild.children[1].firstElementChild.children;

                const deliveringSwitch = orderProductsTable[7].lastElementChild.firstElementChild.firstElementChild;

                deliveringSwitch.disabled = true;

                connection.invoke("RedirectOrderIdWithItsMode", id, mode)
                .catch(function (err) {
                    return console.error(err.toString());
                });
            }
        },
        error: (xhr, status, error) => { }
    });
}


// Deleting an order from an admin view perspective
async function performAdminViewDeletionAsync(id) {
    $.support.cors = true;
    await $.ajax({
        url: `/Order/PerformUserOrAdminViewDeletion/${id}?view=AdminView`,
        type: "GET",
        cache: false,
        success: (result, status, xhr) => {
            if (result == true) {

                const targetInactiveOrderCard = document.getElementById(`inactive-order-card-${id}`);

                targetInactiveOrderCard.remove();
            }
        },
        error: (xhr, status, error) => { }
    });
}

connection.on("SendSpecificOrderIdAsync", (orderId) => {

    const targetAdminActiveOrder = document.getElementById(`order-card-${orderId}`);

    if (targetAdminActiveOrder == null) {
        const targetAdminInactiveOrder = document.getElementById(`inactive-order-card-${orderId}`);
        targetAdminInactiveOrder.remove();
        return 0;
    }

    targetAdminActiveOrder.remove();
});