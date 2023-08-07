
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/order-hub").build();

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("SendOrderAsync", function (currentOrder) {

    const cardsContainer = document.getElementById("cardsContainer");

    const cardWrapper = document.createElement("div");
    cardWrapper.classList.add(["card"]);
    cardWrapper.classList.add(["text-start"]);
    cardWrapper.classList.add(["mb-3"]);
    cardWrapper.style.width = "40%";

    const cardBody = document.createElement("div");
    cardBody.classList.add(["card-body"]);


    const currentOrderNumber = document.getElementsByClassName("ordersCount").length + 1;


    cardBody.innerHTML = `
        <h5 class="card-title">Order <span class="text-muted ordersCount">#${currentOrderNumber}</span></h5>

        <table class="table table-hover align-middle">
            <tbody>
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
                    <td>188.00</td>
                </tr>
                <tr>
                    <td class="fw-bold">Date-Time</td>
                    <td>${currentOrder.initiatingDateAndTime}</td>
                </tr>
                <tr>
                    <td class="fw-bold">Order Status</td>
                    <td>
                        <div>
                            <select class="form-select" aria-label="Order status selection" value="${currentOrder.status}">
                                <option value="Active">Active</option>
                                <option value="Done">Done</option>
                                <option value="Cancelled">Cancelled</option>
                            </select>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="form-check-label fw-bold">Preparing</label>
                    </td>
                    <td>
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" role="checkbox" id="firstFlexSwitchCheck" value="${currentOrder.startedPreparing}">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="form-check-label fw-bold">Delivering</label>
                    </td>
                    <td>
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" role="checkbox" id="secondFlexSwitchCheck" value="${currentOrder.startedDelivering}">
                        </div>
                    </td>
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
});


