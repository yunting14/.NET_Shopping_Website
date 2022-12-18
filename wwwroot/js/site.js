
window.onload = function () {
    checkLoginStatus();
    dbrestore();
    initListeners();
    updateListeners();
    getTotal();
    initiate();
}

//checkLoginStatus

function checkLoginStatus()
{
   
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Login/isLogin");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            if (this.status !== 200) { return; }  //check Http status
            let data = JSON.parse(this.responseText);
            
            if (data.isLogin === false) {
                document.getElementById("Login").style.display = 'block';
                document.getElementById("Logout").style.display = "none";
                document.getElementById("ph-display").style.display = "none";
                document.getElementById("Display-Name").innerHTML = "";
            } else {
                document.getElementById("Login").style.display = "none";
                document.getElementById("Logout").style.display = "block";
                document.getElementById("ph-display").style.display = "block";
                document.getElementById("Display-Name").innerHTML = "Welcome back,"+ " " + data.username;
                getTotal();

            }

        }
    }
    
    xhr.send();
}

function getTotal() {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Browse/ShowQuantity");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            if (this.status !== 200) { return; }  //check Http status
            let data = JSON.parse(this.responseText);

            var cookie = document.cookie.split(';').map(cookie => cookie.split('=')).
                reduce((accumulactor, [key, value]) => ({ ...accumulactor, [key.trim()]: decodeURIComponent(value) }), {})
            var quantity = 0;
            for (let i = 1; i <= 6; i++) {
                if (cookie[i] != null) {
                    quantity = quantity + (+cookie[i]);
                }
            }
            localStorage.setItem("quantity", data.mattTotal + (+quantity));
            initiate();
        }
    }

    xhr.send();
}


//Purchase History and Rating
function dbrestore() {
    dbGetStar();
}

function initListeners() {
    let star = document.getElementsByClassName("star");
    for (let i = 0; i < star.length; i++) {
        star[i].addEventListener('click', onStarClick);
    }
}

function dbGetStar() {
    let ajax = new XMLHttpRequest();
    ajax.open("GET", "/Purchase/GetStarRating");
    ajax.onreadystatechange = function () {
        if (this.readyState == XMLHttpRequest.DONE) {
            if (this.status == 200) {
                onGetStar(JSON.parse(this.responseText));
            }
        }
    }
    ajax.send();
}


function onGetStar(dataList) {
    if (dataList == null) {
        return;
    }

    let Str;
    let starRs;
    for (let i = 0; i < dataList.length; i++) {
        console.log(dataList[i].ProductId);
        console.log(dataList[i].UserId);
        console.log(dataList[i].Rating);
        Str = dataList[i].Rating.toString() + "_" + dataList[i].ProductId.toString();
        starRs = document.getElementsByClassName(Str);
       
        for (let j = 0; j < starRs.length; j++) {
            if (starRs[j] != null) {
                starRs[j].checked = true;
            }
        }

    }

}

function onStarClick(event) {
    let nameStr = event.target.name.toString();
    const nameList = nameStr.split(" ");
    let productId = nameList[1];
    let productRating = event.target.value;
    Str = productRating + "_" + productId;
    const list = document.getElementsByClassName(Str);
    for (let j = 0; j < list.length; j++) {
        if (list[j] != null) {
            list[j].checked = true;
        }
    }
    SetStarRating(productId, productRating);
}

function SetStarRating(productId, productRating) {
    let ajax = new XMLHttpRequest();
    // need to recreate ajax object for each request (cannot re-use)
    ajax.open("POST", "/Purchase/SetStarRating");
    ajax.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    ajax.onreadystatechange = function () {
        if (this.readyState == XMLHttpRequest.DONE) {
            if (this.status == 200) {
                // color string returned from server
                return this.responseText;
                dbrestore();
            }
        }
    }
    ajax.send("ProductId=" + productId + "&Rating=" + productRating);
}

//matt
function initiate() {
    //getTotal();
    const cartIcon = document.querySelector('.fa-cart-arrow-down');

    let local = localStorage.getItem("quantity");

    if (local > 0) {
        cartIcon.classList.add('non-empty');
        let root = document.querySelector(':root');
        root.style.setProperty('--after-content', `"${local}"`);
    }
    if (local == 0) {
        cartIcon.classList.remove('non-empty');
    }

}

function clickCount() {
    const cartIcon = document.querySelector('.fa-cart-arrow-down');
    let local = localStorage.getItem("quantity");
    count = 0 + (+local);
    count++;
    if (count > 0) {
        cartIcon.classList.add('non-empty');
        let root = document.querySelector(':root');
        root.style.setProperty('--after-content', `"${count}"`);
    }
    else if (count == 0) {
        cartIcon.classList.remove('non-empty')
    }

    localStorage.setItem("quantity", count);
}

function cleardata() {

    localStorage.clear();
    initiate();

}

function WaitForClick(event) {
    addToCart(event.toString());
}

function addToCart(productId) {
    let ajax = new XMLHttpRequest();

    ajax.open("POST", "/Browse/AddToCart");
    ajax.setRequestHeader("Content-Type", "application/json; charset=utf8");
    ajax.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            // receive response from server
            if (this.status !== 200) {
                return;
            }
            // convert from JSON string to JavaScript object
            let response = JSON.parse(this.responseText);

        }
    }

    ajax.send(JSON.stringify(productId.toString()));
}

//YT

function updateListeners() {
    let elem = document.getElementsByClassName("quantity-input");
    for (let i = 0; i < elem.length; i++) {
        elem[i].addEventListener('change', UpdateQty, false);
    }
    var quantity = 0;
    for (let i = 1; i <= 6; i++) {

        if (document.getElementById(i) != null) {
            quantity = quantity + (+document.getElementById(i).value);
        }
    }
    //const totalGQ = quantity;
    localStorage.setItem("quantity", quantity);
}

function UpdateQty() {

    
    let productId = event.target.id;
    let updated_elem = document.getElementById(productId);
    let quantity = document.getElementById(productId).value;
    let unitPrice = updated_elem.getAttribute("unitPrice");

    // change sub-total for each product
    let subtotal = document.getElementById("price_" + productId);
    let old_subtotal = subtotal.firstChild.nodeValue;
    let new_subtotal = Number(quantity) * Number(unitPrice);
    subtotal.innerHTML = new_subtotal;
    getTotal();
    // change total amount
    let totalamount = document.getElementById("totalamount");
    let new_total = Number(totalamount.firstChild.nodeValue) - Number(old_subtotal) + Number(new_subtotal);
    totalamount.innerHTML = new_total;
    let userId = updated_elem.getAttribute("userId");
    UpdateCart(userId, productId, quantity);
}

function UpdateCart(userId, productId, quantity) {
    let ajax = new XMLHttpRequest();

    ajax.open("post", "/Cart/UpdateCart");
    ajax.setRequestHeader("Content-Type", "application/json; charset=utf8");

    ajax.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            // receive response from server
            if (this.status !== 200) {
                return;
            }
            getTotal();
            // convert from JSON string to JavaScript object
            let response = JSON.parse(this.responseText);
            // check availability response
            //if (response.isSuccess === true) {
            //    alert("Cart updated!");
            //}
        }
    }

    let data = {
        "UserId": userId,
        "ProductId": productId,
        "Quantity": quantity
    }
    ajax.send(JSON.stringify(data))
}

function totalAmount() {


}