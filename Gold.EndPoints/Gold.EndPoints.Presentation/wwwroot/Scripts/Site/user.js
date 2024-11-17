
//import PhotoSwipeLightbox from 'https://cdnjs.cloudflare.com/ajax/libs/photoswipe/5.4.0/photoswipe-lightbox.esm.min.js';
//import PhotoSwipe from 'https://cdnjs.cloudflare.com/ajax/libs/photoswipe/5.4.0/photoswipe.esm.min.js';


//const leftArrowSVGString = '<svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40" fill="none"><path d="M34.4366 28.3333C31.5549 33.315 26.1687 36.6666 19.9997 36.6666C10.7949 36.6666 3.33301 29.2047 3.33301 20C3.33301 10.7952 10.7949 3.33331 19.9997 3.33331C26.1687 3.33331 31.5549 6.68497 34.4366 11.6666M19.9998 13.3333L13.3331 20M13.3331 20L19.9998 26.6666M13.3331 20H36.6665" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>';
//const rightArrowSVGString = '<svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40" fill="none"><path d="M5.56336 28.3333C8.44511 33.315 13.8313 36.6666 20.0003 36.6666C29.2051 36.6666 36.667 29.2047 36.667 20C36.667 10.7952 29.2051 3.33331 20.0003 3.33331C13.8313 3.33331 8.44511 6.68497 5.56336 11.6666M20.0002 13.3333L26.6669 20M26.6669 20L20.0002 26.6666M26.6669 20H3.33355" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>';
//const lightbox = new PhotoSwipeLightbox({
//    gallery: '#swiper-content',
//    children: 'a',
//    pswpModule: PhotoSwipe,
//    wheelToZoom: true,
//    arrowPrevSVG: leftArrowSVGString,
//    arrowNextSVG: rightArrowSVGString
//});

//lightbox.init();
//let swiper = new Swiper('.swiper', {
//    loop: true,
//    pagination: {
//        el: '.swiper-pagination',
//    },
//    navigation: {
//        nextEl: '.swiper-button-next',
//        prevEl: '.swiper-button-prev',
//    },
//});
//$(".accordion-button").click(function () {
//    console.log()
//    const bodyHeight = $(this).parents('.accordion-item').find('.accordion-body')[0].clientHeight;
//    // console.log($(this).parents('.accordion-item').find('.accordion-body')[0] )
//    if (bodyHeight > 290)
//        $(this).parents('.accordion-item').find('.accordion-collapse')[0].style.overflow = 'auto'
//    else
//        $(this).parents('.accordion-item').find('.accordion-collapse')[0].style.overflow = 'hidden';
//})




function getProductInfo(productId) {
    var formData = new FormData();
    formData.append('productId', productId)
    $.ajax({
        type: "POST",
        url: '/Product/GetProductInfo',
        data: formData,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result != undefined) {
                unSetProductInfoData();
                setProductInfoData(result.Id, result.DefaultPrePayment, result.IsBookmarked, result.Title, result.InstallmentPurchaseOfProduct,
                    result.FinalPrice, result.GoldPrice, result.StonePrice, result.GalleryTitle, result.GalleryDescription,
                    result.Wage, result.Weight, result.WageAmount, result.TaxAmount, result.GalleryProfit, result.GalleryProfitAmount,
                    result.Description, result.HasDiscount, result.Discount, result.DiscountedPrice, result.GalleryFiles, result.GallerySellers);
            }
        },
        error: function (err) {
            console.log(err)
        }
    });
}
$("#btn-rules").click(function () {
    $("#buy-rule-area").hide();
    $("#buy-factor-area").show();
});



function setProductInfoData(productId, defaultPrePayment, isFavorite, title, installmentPurchaseOfProduct,
    finalPrice, goldPrice, stonePrice, galleryTitle, galleryDescription, wage, weight, wageAmount,
    taxAmount, galleryProfit, galleryProfitAmount, description,
    hasDiscount, discount, discountedPrice, mediaGallery, sellerInfoList) {
    //_ProductInfoModal
    document.getElementById("productId").value = productId;
    document.getElementById("prePayment").value = defaultPrePayment.toLocaleString();
    ProductInstallmentsCalculation();
    document.getElementById('productInfoModal_productHeadTitle').setAttribute('data-title', title);
    let shareUrl = `${window.location.origin}/Product/All?p=${productId}`
    document.getElementById('productInfoModal_productHeadTitle').setAttribute('data-href', shareUrl);
    document.getElementById('productInfoModal_nstallmentPurchaseOfProduct').innerHTML = installmentPurchaseOfProduct;
    document.getElementById('productInfoModal_FinalPrice').innerText = finalPrice.toLocaleString();
    document.getElementById('invoiceAmount').innerText = finalPrice.toLocaleString();
    document.getElementById('productInfoModal_GoldPrice').innerText = goldPrice.toLocaleString();
    document.getElementById('productInfoModal_StonePrice').innerText = stonePrice.toLocaleString();
    document.getElementById('productInfoModal_GalleryTitleDesc').innerText = `این محصول توسط گالری ${galleryTitle} ارایه میگردد.`;
    document.getElementById('productInfoModal_GalleryTitle').innerText = galleryTitle;
    document.getElementById('sellerGalleryName').innerText = galleryTitle;
    document.getElementById('productInfoModal_GalleryDescription').innerHTML = galleryDescription;
    document.getElementById('productInfoModal_Wage').innerText = wage;
    document.getElementById('productInfoModal_PWage').innerText = wage;
    document.getElementById('productInfoModal_WageAmount').innerText = wageAmount.toLocaleString();

    document.getElementById('productInfoModal_TaxAmount').innerText = taxAmount.toLocaleString();

    document.getElementById('productInfoModal_GalleryProfitAmount').innerText = galleryProfitAmount.toLocaleString();
    document.getElementById('productInfoModal_GalleryProfit').innerText = galleryProfit;

    if (description.length > 0) {
        document.getElementById('productInfoModal_Description').innerHTML = description;
    } else {
        document.getElementById('productInfoModal_Description').innerHTML = '  <p>توضیحاتی در خصوص این محصول توسط گالری ارایه نشده است .</p>';
    }

    document.getElementById('productInfoModal_Title').innerText = title;

    let productPriceData = '';
    if (hasDiscount) {
        productPriceData = ` <h6 class="position-relative mb-3">
                                    <del>
                                        ${discountedPrice.toLocaleString()}
                                    </del>
                                    <span class="percent-element">
                                        <b>
                                            <i class="fa-solid fa-percent"></i>
                                            <span>
                                               ${discount}
                                            </span>
                                        </b>
                                        <br>
                                        <small>
                                            تخفیف
                                        </small>
                                    </span>
                                    <small>
                                        تومان
                                    </small>
                                </h6>

                                <h6>
                                   ${finalPrice.toLocaleString()}
                                    <small>
                                        تومان
                                    </small>
                                </h6>`;
    } else {
        productPriceData = ` <h6>
        ${finalPrice.toLocaleString()}
                                  <small>
                                      تومان
                                  </small>
                                </h6>`;
    }
    document.getElementById('productInfoModal_PricData').innerHTML = productPriceData;
    document.getElementById('productInfoModal_Weight').innerText = weight;
    document.getElementById('slide-items').innerHTML = '';
    if (mediaGallery.length > 0) {
        for (var i = 0; i < mediaGallery.length; i++) {
            if (mediaGallery[i].IsImage) {
                document.getElementById('slide-items').innerHTML += renderProductImageGallerySlider(mediaGallery[i].FileName);
            } else {
                document.getElementById('slide-items').innerHTML += renderProductVideoGallerySlider(mediaGallery[i].FileName);
            }
        }

    }
    //fill contact us info
    document.getElementById('contactUsSellerInfo').innerHTML = '';
    if (sellerInfoList.length > 0) {
        for (var i = 0; i < sellerInfoList.length; i++) {
            document.getElementById('contactUsSellerInfo').innerHTML += renderContactUsInfo(sellerInfoList[i].ProfileImage, sellerInfoList[i].Name, sellerInfoList[i].Phone);
        }
    }

    //bookmark
    document.getElementById('favoriteSection').innerHTML = renderFavoriteSection(productId, isFavorite);

    // $('#productInfo').modal({ backdrop: "static ", keyboard: false },'show');
    setSliders();
    $('#productInfo').modal('show');

}

function unSetProductInfoData() {
    //_ProductInfoModal
    document.getElementById('insatllmentCount').value = 3;
    document.getElementById("productId").value = 0;
    document.getElementById("prePayment").value = 0;
    document.getElementById('productInfoModal_productHeadTitle').setAttribute('data-title', '');
    document.getElementById('productInfoModal_nstallmentPurchaseOfProduct').innerHTML = '';
    document.getElementById('productInfoModal_FinalPrice').innerText = '';
    document.getElementById('productInfoModal_GoldPrice').innerText = '';
    document.getElementById('productInfoModal_StonePrice').innerText = '';
    document.getElementById('productInfoModal_GalleryTitleDesc').innerText = '';
    document.getElementById('productInfoModal_GalleryTitle').innerText = '';
    document.getElementById('sellerGalleryName').innerText = '';
    document.getElementById('productInfoModal_GalleryDescription').innerHTML = '';
    document.getElementById('productInfoModal_Wage').innerText = '';
    document.getElementById('productInfoModal_PWage').innerText = '';
    document.getElementById('productInfoModal_WageAmount').innerText = '';
    document.getElementById('productInfoModal_TaxAmount').innerText = '';
    document.getElementById('productInfoModal_GalleryProfitAmount').innerText = '';
    document.getElementById('productInfoModal_GalleryProfit').innerText = '';
    document.getElementById('productInfoModal_Description').innerHTML = '';
    document.getElementById('productInfoModal_Title').innerText = '';
    document.getElementById('productInfoModal_PricData').innerHTML = '';
    document.getElementById('productInfoModal_Weight').innerText = '';
    //fill media gallery
    // document.getElementById('swiper-wrapper-area').innerHTML = '';
    // document.getElementById('image-modal-body').innerHTML = '';
    // document.getElementById('slides').innerHTML = '';
    //fill contact us info
    document.getElementById('contactUsSellerInfo').innerHTML = '';

    //bookmark
    document.getElementById('favoriteSection').innerHTML = '';
}

function renderFavoriteSection(productId, isFavorite) {
    if (isFavorite) {
        return ` <i id='favoritState' onclick="unFavorite('${productId}',true)" class="fa fa-bookmark ms-3"></i>`
    } else {
        return ` <i id='favoritState' onclick="addToFavorite('${productId}')" class="fa-light fa-bookmark ms-3"></i>`
    }
}

function renderProductImageGallerySlider(fileName) {
    //return `<div class="swiper-slide">
    //                             <img src="/Product/MediaGallery/${fileName}" alt="">
    //                         </div>`;
    return `<li > <img src="/Product/MediaGallery/${fileName}" alt=""></li>`;
}
function renderProductVideoGallerySlider(fileName) {
    //return `<div class="swiper-slide">
    //                           <video width="800" height="600" controls>
    //                                <source src="/Product/MediaGallery/${fileName}" type="video/mp4">
    //                           </video>
    //                         </div>`;
    return `<li style="background: transparent;"> 
                              <video controls>
                                   <source src="/Product/MediaGallery/${fileName}" type="video/mp4">
                              </video  class='slide' style='display: none;'>
                           </li> `;
}
function renderProductImageGallery(fileName) {
    return `<div class="swiper-slide">
                                           <a itemprop="contentUrl"
                                               href="/Product/MediaGallery/${fileName}"
                                               data-pswp-width="500"
                                               data-pswp-height="500">
                                                <img src="/Product/MediaGallery/${fileName}" alt="">
                                            </a>
                                        </div>`;
}


function renderProductVideoGallery(fileName) {
    return `<div class="swiper-slide">
                                           <a itemprop="contentUrl"
                                               href="/Product/MediaGallery/${fileName}"
                                               data-pswp-width="500"
                                               data-pswp-height="500">
                                               <video width="320" height="240" controls>
                                                    <source src="/Product/MediaGallery/${fileName}" type="video/mp4">
                                                </video>
                                            </a>
                                        </div>`;
}

function renderContactUsInfo(profileImageName, sellerName, phone) {
    return `  <tr>
                        <td>
                            <a href="tel:${phone}">
                              <i class="fa-regular fa-phone-volume text-primary"></i>
                            </a>
                        </td>
                        <td>
                            <img width="40" class="object-fit-cover rounded-circle"
                                 src="/Product/SellerImage/${profileImageName}" alt="${sellerName}">
                           ${sellerName}
                        </td>
                        <td dir="ltr">
                            <a href="tel:${phone}">
                                ${phone}
                            </a>
                        </td>
                    </tr>`;
}


function ProductInstallmentsCalculation() {
    setTimeout(function () {
        let productId = document.getElementById("productId").value;
        let prePayment = document.getElementById('prePayment').value;
        let insatllmentCount = parseInt(document.getElementById('insatllmentCount').value);
        if (prePayment.length > 3) {
            var formData = new FormData();
            formData.append('ProductId', productId)
            formData.append('PrePayment', prePayment)
            formData.append('InstallmentCount', insatllmentCount)
            $.ajax({
                type: "POST",
                url: '/Product/ProductInstallmentsCalculation',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined) {
                        if (response.IsSuccess) {
                            document.getElementById('installmentAmount').innerText = response.Data.InstallmentAmount.toLocaleString();
                        } else {
                            ShowToastAlert(response.Type, response.Message)
                        }
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });
        }
    }, 1000);


}

function setSliders() {
    slideCount = $('#slider ul li').length;
    slideWidth = $('#slider ul li').width();
    slideHeight = $('#slider ul li').height();
    sliderUlWidth = slideCount * slideWidth;

    $('#slider').css({ width: slideWidth, height: slideHeight });

    $('#slider ul').css({ width: sliderUlWidth, marginLeft: - slideWidth });

    $('#slider ul li:last-child').prependTo('#slider ul');
}
jQuery(document).ready(function ($) {
    //$('#checkbox').change(function () {
    //    setInterval(function () {
    //        moveRight();
    //    }, 5000);
    //});
    setSliders();
});

function moveLeft() {
    $('#slider ul').animate({
        left: + slideWidth
    }, 200, function () {
        $('#slider ul li:last-child').prependTo('#slider ul');
        $('#slider ul').css('left', '');
    });
};

function moveRight() {
    $('#slider ul').animate({
        left: - slideWidth
    }, 200, function () {
        $('#slider ul li:first-child').appendTo('#slider ul');
        $('#slider ul').css('left', '');
    });
};

$('a.control_prev').click(function () {
    moveLeft();
});

$('a.control_next').click(function () {
    moveRight();
});