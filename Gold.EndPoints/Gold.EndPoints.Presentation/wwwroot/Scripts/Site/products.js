
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
const acc1 = `<div class="accordion-item" id="installmentPurchase">
										<h2 class="accordion-header">
											<button class="accordion-button collapsed" type="button"
													data-bs-toggle="collapse"
													data-bs-target="#productInfoAccordionArea-collapseOne"
													aria-expanded="false" aria-controls="flush-collapseOne">
												خرید محصول ( قسطی )
											</button>
										</h2>
										<div id="productInfoAccordionArea-collapseOne"
											 class="accordion-collapse collapse"
											 data-bs-parent="#productInfoAccordionArea">
											<div class="accordion-body px-1 accordion-body-white text-justify">
												<div style="display: block" id="buy-rule-area" class="buy-rule px-3">
													<div id="productInfoModal_nstallmentPurchaseOfProduct">
													</div>
													<h6 class="text-center">
														<button id="btn-rules" class="btn btn-primary rounded-pill">
															<i class="fa-solid fa-check"></i>
															شرایط را خواندم و قبول دارم
														</button>
													</h6>
												</div>
												<div style="display: none" id="buy-factor-area" class=" table-responsive">
													<table class="table factor-table mb-0">
														<tr>
															<td colspan="1">
																<h6 class="small">
																	مبلغ تمام شده فاکتور شما :
																</h6>
															</td>
															<td class="text-start">
																<span class="btn-outline-gray-gold px-2 py-1">
																	<span id="invoiceAmount"></span>
																	<small>
																		تومان
																	</small>
																</span>
															</td>
														</tr>

														<tr>
															<td colspan="1">
																<h6 class="small">
																	مبلغ پیش پرداخت را وارد کنید :
																</h6>
																@*     <small class="text-danger error-input">
																* مبلغ وارد شده کمتر از حد مجاز است .
																</small>*@
															</td>
															<td class="text-start">
																<span class="btn-outline-gray-gold px-2 py-1">
																	<input type="text" id="prePayment" onchange="ProductInstallmentsCalculation()" onkeyup="monyFormat(this)" onkeypress="return safeOnlyNumber($(this),event)" class="w-50 number-input">
																	<small>
																		تومان
																	</small>
																</span>
															</td>
														</tr>

														<tr>
															<td colspan="1">
																<h6 class="small">
																	تعداد اقساط را انتخاب کنید :
																</h6>
															</td>
															<td class="text-start">
																<div dir="ltr"
																	 class="btn-outline-gray-gold number-input-group input-group-sm d-flex w-100">
																	<button onclick="ProductInstallmentsCalculation()" class="btn btn-link btn-minus" type="button"
																			id="button-addon1">
																		<i class="fa-solid fa-minus-circle"></i>
																	</button>
																	<small>ماهه</small>
																	<input data-min="3"
																		   readonly
																		   data-max="12"
																		   data-len="2"
																		   type="text"
																		   value="3"
																		   class="form-control number-add-minus number"
																		   id="insatllmentCount"
																		   aria-label="Amount (to the nearest dollar)">
																	<button onclick="ProductInstallmentsCalculation()" class="btn btn-link btn-plus btn-plus-primary " type="button"
																			id="button-addon2">
																		<i class="fa-solid fa-plus-circle"></i>
																	</button>
																</div>
															</td>
														</tr>

														<tr>
															<td colspan="1">
																<h6 class="small">
																	مبلغ نهایی هر قسط :
																</h6>
															</td>
															<td class="text-start">
																<span class="btn-outline-gray-gold px-2 py-1">
																	<span id="installmentAmount"></span>
																	<small>
																		تومان
																	</small>
																</span>
															</td>
														</tr>
														<tr>
															<td colspan="2" class="border-bottom-0">
																<p class="text-center buy-contact-info mb-0">
																	جهت خرید محصول تماس بگیرید
																	<br>
																	<button data-bs-toggle="modal"
																			data-bs-target="#contactInfo"
																			class="btn btn-outline-gray-gold rounded-pill mt-2">
																		اطلاعات تماس
																		<i class="fa-regular fa-phone-volume"></i>
																	</button>
																</p>
															</td>
														</tr>

													</table>
												</div>
											</div>
										</div>
									</div>`;

const acc2 = `<div class="accordion-item">
										<h2 class="accordion-header">
											<button class="accordion-button collapsed" type="button"
													data-bs-toggle="collapse"
													data-bs-target="#productInfoAccordionArea-collapseTwo"
													aria-expanded="false" aria-controls="flush-collapseTwo">
												خرید محصول ( نقدی )
											</button>
										</h2>
										<div id="productInfoAccordionArea-collapseTwo"
											 class="accordion-collapse collapse"
											 data-bs-parent="#productInfoAccordionArea">
											<div class="accordion-body accordion-body-white">
												<h6 class="fw-normal" id="productInfoModal_GalleryTitleDesc">
												</h6>
												<hr>
												<div id="productInfoModal_GalleryDescription">
												</div>
												@*<h6>آدرس :</h6>
												<p>
												شیراز - لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ
												</p>*@
												<hr>
												<div class="buy-contact-info">
													<p class="text-center buy-contact-info mb-0">
														جهت خرید محصول تماس بگیرید
														<br>
														<button data-bs-toggle="modal"
																data-bs-target="#contactInfo"
																class="btn btn-outline-gray-gold rounded-pill mt-2">
															اطلاعات تماس
															<i class="fa-regular fa-phone-volume"></i>
														</button>
													</p>
												</div>

											</div>
										</div>
									</div>`;

const acc3 = `<div class="accordion-item">
										<h2 class="accordion-header">
											<button class="accordion-button collapsed" type="button"
													data-bs-toggle="collapse"
													data-bs-target="#productInfoAccordionArea-collapseThree"
													aria-expanded="false" aria-controls="flush-collapseThree">
												توضیحات قیمت
											</button>
										</h2>
										<div id="productInfoAccordionArea-collapseThree"
											 class="accordion-collapse collapse"
											 data-bs-parent="#productInfoAccordionArea">
											<div class="accordion-body  table-responsive accordion-body-white text-justify">
												<table class="table factor-table mb-0">
													<tr>
														<td colspan="2">
															<h6 class="small">
																قیمت نهایی محصول :
															</h6>
														</td>
														<td class="text-start">
															<span class="btn-outline-gray-gold px-2 py-1">
																<span id="productInfoModal_FinalPrice">
																	25,500,000
																</span>
																<small>
																	تومان
																</small>
															</span>
														</td>
													</tr>

													<tr>
														<td colspan="2">
															<h6 class="small">
																ارزش طلای محصول :
															</h6>
														</td>
														<td class="text-start">
															<span class="btn-outline-gray-gold px-2 py-1">
																<span id="productInfoModal_GoldPrice">

																</span>
																<small>
																	تومان
																</small>
															</span>
														</td>
													</tr>
													<tr>
														<td colspan="2">
															<h6 class="small">
																ارزش سنگ :
															</h6>
														</td>
														<td class="text-start">
															<span class="btn-outline-gray-gold px-2 py-1">
																<span id="productInfoModal_StonePrice">

																</span>
																<small>
																	تومان
																</small>
															</span>
														</td>
													</tr>

													<tr>
														<td colspan="2">
															<h6 class="small">
																اجرت ساخت :
																<small class="text-gray_middle">
																	<span id="productInfoModal_Wage">
																	</span> درصد
																</small>
															</h6>
														</td>
														<td class="text-start">
															<span class="btn-outline-gray-gold px-2 py-1">
																<span id="productInfoModal_WageAmount">

																</span>
																<small>
																	تومان
																</small>
															</span>
														</td>
													</tr>

													<tr>
														<td colspan="2">
															<h6 class="small">
																سود گالری :
																<small class="text-gray_middle">
																	<span id="productInfoModal_GalleryProfit">

																	</span> درصد
																</small>
															</h6>
														</td>
														<td class="text-start">
															<span class="btn-outline-gray-gold px-2 py-1">
																<span id="productInfoModal_GalleryProfitAmount"></span>
																<small>
																	تومان
																</small>
															</span>
														</td>
													</tr>


													<tr>
														<td colspan="2">
															<h6 class="small">
																مالیات :
															</h6>
														</td>
														<td class="text-start">
															<span class="btn-outline-gray-gold px-2 py-1">
																<span id="productInfoModal_TaxAmount"></span>
																<small>
																	تومان
																</small>
															</span>
														</td>
													</tr>
													<tr>
														<td colspan="3" class="border-bottom-0">
															<p class="text-center m-0 pt-2 text-small small fw-bold">
																محاسبه قیمت بر اساس قیمت قانون حذف مالیات از اصل طلا
															</p>
														</td>
													</tr>

												</table>
											</div>
										</div>
									</div>`;

const acc4 = `<div class="accordion-item">
										<h2 class="accordion-header">
											<button class="accordion-button collapsed" type="button"
													data-bs-toggle="collapse"
													data-bs-target="#productInfoAccordionArea-collapseFour"
													aria-expanded="false" aria-controls="flush-collapseFour">
												توضیحات محصول
											</button>
										</h2>
										<div id="productInfoAccordionArea-collapseFour"
											 class="accordion-collapse collapse"
											 data-bs-parent="#productInfoAccordionArea">
											<div class="accordion-body accordion-body-white">
												<div id="productInfoModal_Description">
												</div>

											</div>
										</div>
									</div>`;


let sortBy = '';
let categories = [];
let term = '';
let categoryList = '';

// active page
let page = 1;
// total page want to scroll
let scroll_page = 3;
// for final requeset
let final = false;

// for preventing a service to call when another request is running
let block = false;
$(document).ready(function () {
    categories = getSelectedCategory();

    $("#loading").hide()
    $("#pagiante").hide()
    //$("#listArea").html('');
    // get Element for first time
    //handelRequest(page);
    setActivePaginate();
    openProductModal();
})

$(window).on("scroll", async function () {
    let windowHeight = $(window).height();
    let scrollTop = $(window).scrollTop();
    let divOffset = $("#listArea").offset().top;
    let divHeight = $("#listArea").height();

    if (page < scroll_page && !final && !block) {
        if (scrollTop + windowHeight >= divOffset + divHeight) {
            page = page + 1;
            await handelRequest(page)
        }
    }

});


$('.page-item').click(async function () {
    const el = $(this);
    final = true;
    block = true;
    let value = el.text().trim();
    page = value;

    setActivePaginate(el);

    $("html, body").animate({
        scrollTop: $("#listArea").offset().top - 100
    }, 300);
    $("#listArea").html('');
    await handelRequest(page)
})

async function handelRequest(pageNumber) {
    block = true;
    $("#loader").show();
    return new Promise(resolve => setTimeout(() => {
        var formData = new FormData();
        formData.append('SortBy', sortBy)
        formData.append('Term', term)
        formData.append('Page', pageNumber)
        if (categories.length > 0) {
            for (var i = 0; i < categories.length; i++) {
                formData.append(`categories`, parseInt(categories[i]))
            }
        }
        $.ajax({
            type: "POST",
            url: '/Product/GetProducts',
            data: formData,
            contentType: false,
            processData: false,
            success: function (items) {
                if (items != undefined) {
                    renderItem(items);
                    block = false;
                    $("#loader").hide()
                    resolve(items);

                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    }, 10))
}
function setActivePaginate(el = null) {
    $('.page-item').each(function (element) {
        $(this).parent().removeClass('active');
        if (!el && $(this).text().trim() == scroll_page) {
            $(this).parent().addClass('active');
        }
    })
    if (el)
        el.parent().addClass('active');
}



//get products

function getSelectedCategory() {
    let result = [];
    //categories = [];
    var markedCheckbox = document.getElementsByName('CategoryFilter');
    console.log(result)
    for (var checkbox of markedCheckbox) {
        if (checkbox.checked) {
            result.push(checkbox.value)
            //  checkbox.checked = true;
        }
    }
    return result;
}

function setCategory(el) {
    var productCategories = document.getElementsByName('categories');
    for (var i = 0; i < productCategories.length; i++) {
        if (productCategories[i].value == el.value) {
            productCategories[i].checked = el.checked;
        }
    }
    submitFilterForm();
}

function setSearchTerm(el) {
    document.getElementById('searchTerm').value = el.value;
}

$("#searchTermMobile").keyup(function (event) {
    if (event.keyCode === 13) {
        submitFilterForm();
    }
});

//attach proucts to last items
function renderItem(data) {
    let html = null;
    if (page >= scroll_page) {
        $("#pagiante").show();
        block = true;
    }

    data.forEach(el => {
        html = `<div dir="rtl" class="col-4 col-lg-3">
                     <div class="product ${el.IsSold ? 'saled' : ''}" data-bs-toggle="modal" data-bs-target="#productInfo">
                         <img onclick="getProductInfo('${el.Id}')" class="product-img" src="/Product/Image/${el.ImageName}" alt="${el.Title}">
                     </div>
                </div>`;
        $("#listArea").append(html);
    })
}

function getProductsByFilter(page, category, sortby) {
    let filterUrl = `/Product/All/${page}/${category}/${sortby}`;
    $.getJSON(filterUrl, function (items) {
        renderItem(items);
        block = false;
        $("#loader").hide()
        resolve(items);
    })
}

function filterBySort(value) {
    document.getElementById('sortBy').value = value;
    sortBy = value;
    submitFilterForm();
}

function submitFilterForm() {
    document.getElementById('filterForm').submit();
}

$("#searchTerm").keyup(function (event) {
    if (event.keyCode === 13) {
        submitFilterForm();
    }
});

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
                    result.Description, result.HasDiscount, result.Discount, result.DiscountedPrice, result.GalleryFiles, result.GallerySellers, result.HasInstallmentSale);
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
    hasDiscount, discount, discountedPrice, mediaGallery, sellerInfoList, hasInstallmentSale) {
    //_ProductInfoModal
    document.getElementById("productId").value = productId;
    document.getElementById("prePayment").value = defaultPrePayment.toLocaleString();

    if (hasInstallmentSale) {
        document.getElementById('installmentPurchase').style.display = '';
        ProductInstallmentsCalculation();
    } else {
        document.getElementById('installmentPurchase').style.display = 'none';
    }
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
    //fill media gallery
    document.getElementById('product-image-carousel-inner').innerHTML = '';
    document.getElementById('product-image-carousel-indicators').innerHTML = '';
    document.getElementById('product-single-image-carousel-inner').innerHTML = '';
    document.getElementById('product-single-image-carousel-indicators').innerHTML = '';

    if (mediaGallery.length > 0) {
        document.getElementById('product-image-carousel-indicators').innerHTML = '';
        document.getElementById('product-single-image-carousel-indicators').innerHTML = '';

        document.getElementById('product-image-carousel-indicators').innerHTML +=
            `<button class="carousel-control-next" id="product-image-carousel-control-next" type="button" data-bs-target="#product-image-carousel" 
            onclick="slidersNavItemArrow(0,true);" data-bs-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
				<span class="visually-hidden">Next</span>
            </button>`;

        document.getElementById('product-single-image-carousel-indicators').innerHTML +=
            `<button class="carousel-control-next" id="product-single-image-carousel-control-next" type="button" data-bs-target="#product-single-image-carousel"
            onclick="singleSlidersNavItem(0,true);" data-bs-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
				<span class="visually-hidden">Next</span>
            </button>`;

        for (var i = 0; i < mediaGallery.length; i++) {
            if (i === 0) {
                document.getElementById('product-image-carousel-indicators').innerHTML +=
                    `<button type="button" data-bs-target="#product-image-carousel" onclick="slidersNavItem(${i});" 
                    data-bs-slide-to="${i}" class="active" aria-current="true" aria-label="Slide ${i + 1}"></button>`;

                document.getElementById('product-single-image-carousel-indicators').innerHTML +=
                    `<button type="button" data-bs-target="#product-single-image-carousel" onclick="singleSlidersNavItem(${i});" 
                    data-bs-slide-to="${i}" class="active" aria-current="true" aria-label="Slide ${i + 1}"></button>`;
            } else {
                document.getElementById('product-image-carousel-indicators').innerHTML +=
                    `<button type="button" data-bs-target="#product-image-carousel" onclick="slidersNavItem(${i});" 
                    data-bs-slide-to="${i}" aria-label="Slide ${i + 1}"></button>`;

                document.getElementById('product-single-image-carousel-indicators').innerHTML +=
                    `<button type="button" data-bs-target="#product-single-image-carousel" onclick="singleSlidersNavItem(${i});"
                    data-bs-slide-to="${i}" aria-label="Slide ${i + 1}"></button>`;
            }

            if (mediaGallery[i].IsImage) {
                document.getElementById('product-image-carousel-inner').innerHTML += renderProductImageGalleryCarousel(mediaGallery[i].FileName, (i === 0) ? true : false, i);

                document.getElementById('product-single-image-carousel-inner').innerHTML += renderProductSingleImageGalleryCarousel(mediaGallery[i].FileName, (i === 0) ? true : false);
            } else {
                document.getElementById('product-image-carousel-inner').innerHTML += renderProductVideoGalleryCarousel(mediaGallery[i].FileName, (i === 0) ? true : false, i);

                document.getElementById('product-single-image-carousel-inner').innerHTML += renderProductSingleVideoGalleryCarousel(mediaGallery[i].FileName, (i === 0) ? true : false);
            }
        }

        document.getElementById('product-image-carousel-indicators').innerHTML +=
            `<button class="carousel-control-prev" id="product-image-carousel-control-prev" type="button" data-bs-target="#product-image-carousel" 
            onclick="slidersNavItemArrow(0,false);" data-bs-slide="prev">
				<span class="carousel-control-prev-icon" aria-hidden="true"></span>
			    <span class="visually-hidden">Previous</span>
			</button>`;

        document.getElementById('product-single-image-carousel-indicators').innerHTML +=
            `<button class="carousel-control-prev" id="product-single-image-carousel-control-prev" type="button" data-bs-target="#product-single-image-carousel"
            onclick="singleSlidersNavItemArrow(0,false);" data-bs-slide="prev">
				<span class="carousel-control-prev-icon" aria-hidden="true"></span>
			    <span class="visually-hidden">Previous</span>
			</button>`;
    }

    // document.getElementById('swiper-wrapper-area').innerHTML = '';
    //document.getElementById('image-modal-body').innerHTML = '';
    //document.getElementById('slides').innerHTML = '';
    //document.getElementById('slide-items').innerHTML = '';
    //if (mediaGallery.length > 0) {
    //    for (var i = 0; i < mediaGallery.length; i++) {
    //        if (mediaGallery[i].IsImage) {
    //            // document.getElementById('swiper-wrapper-area').innerHTML += renderProductImageGallery(mediaGallery[i].FileName);
    //            // document.getElementById('image-modal-body').innerHTML += renderProductImageGallerySlider(mediaGallery[i].FileName);
    //            document.getElementById('slide-items').innerHTML += renderProductImageGallerySlider(mediaGallery[i].FileName);
    //        } else {
    //            //document.getElementById('swiper-wrapper-area').innerHTML += renderProductVideoGallery(mediaGallery[i].FileName);
    //            //document.getElementById('image-modal-body').innerHTML += renderProductVideoGallerySlider(mediaGallery[i].FileName);
    //            document.getElementById('slide-items').innerHTML += renderProductVideoGallerySlider(mediaGallery[i].FileName);
    //        }
    //    }

    //}
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
    //setSliders();
    $('#productInfo').modal('show');
}

$('.accordion-button').click(function () {
    if (!this.classList.contains('collapsed')) {
        const h = $('#productInfo .modal-dialog').height();
        $('#productInfo .modal-body').animate({ scrollTop: h }, "slow");
    }
});

function showSingleImageModal(i) {
    singleSlidersNavItem(i);
    const items = $('.product-single-image-slider-item');
    $('.product-single-image-slider-item').removeClass('active');
    items[i].classList += ' active';

    //clearInterval(myInterval);
}

function slidersNavItemArrow(num, Add) {
    const items = $('#product-image-carousel-indicators > button:not(:first-child):not(:last-child)');
    var length = items.length;
    if (Add) {
        if (num === 0) {
            num = (length - 1);
            $('#product-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
            items[num].classList += ' active';
            $('#product-image-carousel-control-next').attr('onclick', `slidersNavItemArrow(${num},true);`);
            $('#product-image-carousel-control-prev').attr('onclick', `slidersNavItemArrow(${num},false);`);
        }
        else {
            num--;
            $('#product-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
            items[num].classList += ' active';
            $('#product-image-carousel-control-next').attr('onclick', `slidersNavItemArrow(${num},true);`);
            $('#product-image-carousel-control-prev').attr('onclick', `slidersNavItemArrow(${num},false);`);
        }
    }
    else {
        if (num === (length - 1)) {
            num = 0;
            $('#product-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
            items[num].classList += ' active';
            $('#product-image-carousel-control-next').attr('onclick', `slidersNavItemArrow(${num},true);`);
            $('#product-image-carousel-control-prev').attr('onclick', `slidersNavItemArrow(${num},false);`);
        }
        else {
            num++;
            $('#product-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
            items[num].classList += ' active';
            $('#product-image-carousel-control-next').attr('onclick', `slidersNavItemArrow(${num},true);`);
            $('#product-image-carousel-control-prev').attr('onclick', `slidersNavItemArrow(${num},false);`);
        }
    }
}

function singleSlidersNavItemArrow(num, Add) {
    const items = $('#product-single-image-carousel-indicators > button:not(:first-child):not(:last-child)');
    var length = items.length;
    if (Add) {
        if (num === 0) {
            num = (length - 1);
            $('#product-single-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
            items[num].classList += ' active';
            $('#product-single-image-carousel-control-next').attr('onclick', `singleSlidersNavItemArrow(${num},true);`);
            $('#product-single-image-carousel-control-prev').attr('onclick', `singleSlidersNavItemArrow(${num},false);`);
        }
        else {
            num--;
            $('#product-single-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
            items[num].classList += ' active';
            $('#product-single-image-carousel-control-next').attr('onclick', `singleSlidersNavItemArrow(${num},true);`);
            $('#product-single-image-carousel-control-prev').attr('onclick', `singleSlidersNavItemArrow(${num},false);`);
        }
    }
    else {
        if (num === (length - 1)) {
            num = 0;
            $('#product-single-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
            items[num].classList += ' active';
            $('#product-single-image-carousel-control-next').attr('onclick', `singleSlidersNavItemArrow(${num},true);`);
            $('#product-single-image-carousel-control-prev').attr('onclick', `singleSlidersNavItemArrow(${num},false);`);
        }
        else {
            num++;
            $('#product-single-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
            items[num].classList += ' active';
            $('#product-single-image-carousel-control-next').attr('onclick', `singleSlidersNavItemArrow(${num},true);`);
            $('#product-single-image-carousel-control-prev').attr('onclick', `singleSlidersNavItemArrow(${num},false);`);
        }
    }
}

function slidersNavItem(num) {
    const items = $('#product-image-carousel-indicators > button:not(:first-child):not(:last-child)');
    $('#product-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
    items[num].classList += ' active';
    var length = items.length;

    if (num === (length - 1)) {
        $('#product-image-carousel-control-next').attr('onclick', `slidersNavItemArrow(${num - 1},true);`);
        $('#product-image-carousel-control-prev').attr('onclick', `slidersNavItemArrow(${0},false);`);
    }
    else if (num === 0) {
        $('#product-image-carousel-control-next').attr('onclick', `slidersNavItemArrow(${length - 1},true);`);
        $('#product-image-carousel-control-prev').attr('onclick', `slidersNavItemArrow(${num + 1},false);`);
    }
    else {
        $('#product-image-carousel-control-next').attr('onclick', `slidersNavItemArrow(${num - 1},true);`);
        $('#product-image-carousel-control-prev').attr('onclick', `slidersNavItemArrow(${num + 1},false);`);
    }
}

function singleSlidersNavItem(num) {
    const items = $('#product-single-image-carousel-indicators > button:not(:first-child):not(:last-child)');
    $('#product-single-image-carousel-indicators > button:not(:first-child):not(:last-child)').removeClass('active');
    items[num].classList += ' active';
    var length = items.length;

    if (num === (length - 1)) {
        $('#product-single-image-carousel-control-next').attr('onclick', `singleSlidersNavItem(${num - 1},true);`);
        $('#product-single-image-carousel-control-prev').attr('onclick', `singleSlidersNavItem(${0},false);`);
    }
    else if (num === 0) {
        $('#product-single-image-carousel-control-next').attr('onclick', `singleSlidersNavItem(${length - 1},true);`);
        $('#product-single-image-carousel-control-prev').attr('onclick', `singleSlidersNavItem(${num + 1},false);`);
    }
    else {
        $('#product-single-image-carousel-control-next').attr('onclick', `singleSlidersNavItem(${num - 1},true);`);
        $('#product-single-image-carousel-control-prev').attr('onclick', `singleSlidersNavItem(${num + 1},false);`);
    }
}

function unSetProductInfoData() {
    //_ProductInfoModal

    $('.accordion-button').addClass('collapsed');
    $('.accordion-collapse.collapse').removeClass('show');
    $('.accordion-button').attr('aria-expanded', 'false');//aria-expanded="true"
    $('.accordion-button').attr('height', '0');
    $("#buy-rule-area").show();
    $("#buy-factor-area").hide();

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

    document.getElementById('productInfoAccordionArea-collapseTwo').classList.remove("show");
    document.getElementById('productInfoAccordionArea-collapseThree').classList.remove("show");
    document.getElementById('productInfoAccordionArea-collapseFour').classList.remove("show");
    document.getElementById('productInfoAccordionArea-collapseFour').classList.remove("show");
}

function renderFavoriteSection(productId, isFavorite) {
    if (isFavorite) {
        return ` <i id='favoritState' onclick="unFavorite('${productId}')" class="fa fa-bookmark ms-3"></i>`
    } else {
        return ` <i id='favoritState' onclick="addToFavorite('${productId}')" class="fa-light fa-bookmark ms-3"></i>`
    }
}

function renderProductImageGalleryCarousel(fileName, isFirst, num) {
    if (isFirst) {
        return `<div class="carousel-item active product-image-slider-item" data-bs-toggle="modal" data-bs-target="#product-single-image-modal" 
                onclick="showSingleImageModal(${num})">
				    <img src="/Product/MediaGallery/${fileName}" class="d-block w-100 img-fluid hand" alt="">
                </div>`
    }
    else {
        return `<div class="carousel-item product-image-slider-item" data-bs-toggle="modal" data-bs-target="#product-single-image-modal"
                onclick="showSingleImageModal(${num})">
				    <img src="/Product/MediaGallery/${fileName}" class="d-block w-100 img-fluid hand" alt="">
                </div>`
    }
}

function renderProductVideoGalleryCarousel(fileName, isFirst, num) {
    if (isFirst) {
        return `<div class="carousel-item active product-image-slider-item hand" data-bs-toggle="modal" data-bs-target="#product-single-image-modal"
                onclick="showSingleImageModal(${num})">
                    <video controls>
                        <source src="/Product/MediaGallery/${fileName}" type="video/mp4">
                    </video  class='slide' style='display: none;'>
                </div>`;
    }
    else {
        return `<div class="carousel-item product-image-slider-item hand" data-bs-toggle="modal" data-bs-target="#product-single-image-modal"
                onclick="showSingleImageModal(${num})">
                    <video controls>
                         <source src="/Product/MediaGallery/${fileName}" type="video/mp4">
                    </video  class='slide' style='display: none;'>
                </div>`;
    }
}

function renderProductSingleImageGalleryCarousel(fileName, isFirst) {
    if (isFirst) {
        return `<div class="carousel-item active product-single-image-slider-item hand">
				    <img src="/Product/MediaGallery/${fileName}" class="d-block w-100 img-fluid hand" alt="">
                </div>`
    }
    else {
        return `<div class="carousel-item product-single-image-slider-item hand">
				    <img src="/Product/MediaGallery/${fileName}" class="d-block w-100 img-fluid hand" alt="">
                </div>`
    }
}

function renderProductSingleVideoGalleryCarousel(fileName, isFirst) {
    if (isFirst) {
        return `<div class="carousel-item active product-single-image-slider-item hand">
                    <video controls>
                        <source src="/Product/MediaGallery/${fileName}" type="video/mp4">
                    </video  class='slide' style='display: none;'>
                </div>`;
    }
    else {
        return `<div class="carousel-item product-single-image-slider-item hand">
                    <video controls>
                         <source src="/Product/MediaGallery/${fileName}" type="video/mp4">
                    </video  class='slide' style='display: none;'>
                </div>`;
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
    let profileImage = '';
    if (profileImageName.length > 0) {
        profileImage = ` <img width="40" class="object-fit-cover rounded-circle"
                                 src="/Product/SellerImage/${profileImageName}" alt="${sellerName}">`;
    }
    return `  <tr>
                        <td>
                            <a href="tel:${phone}">
                              <i class="fa-regular fa-phone-volume text-primary"></i>
                            </a>
                        </td>
                        <td>
                           ${profileImage}
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
    }, 100);


}

function openProductModal() {
    try {
        var items = new URLSearchParams(window.location.search);
        let productId = items.get('p');
        if (productId.length > 0) {
            getProductInfo(productId);
        }
    } catch (e) {

    }
}




//var slideCount = 0;
//var slideWidth = 0;
//var slideHeight = 0;
//var sliderUlWidth = 0;
//slider 2
//function setSliders() {
//    slideCount = $('#slider ul li').length;
//    slideWidth = $('#slider ul li').width();
//    slideHeight = $('#slider ul li').height();
//    sliderUlWidth = slideCount * slideWidth;

//    $('#slider').css({ width: slideWidth, height: slideHeight });

//    $('#slider ul').css({ width: sliderUlWidth, marginLeft: - slideWidth });

//    $('#slider ul li:last-child').prependTo('#slider ul');
//}
//jQuery(document).ready(function ($) {
    //$('#checkbox').change(function () {
    //    setInterval(function () {
    //        moveRight();
    //    }, 5000);
    //});
    //setSliders();
//});
//function moveLeft() {
//    $('#slider ul').animate({
//        left: + slideWidth
//    }, 200, function () {
//        $('#slider ul li:last-child').prependTo('#slider ul');
//        $('#slider ul').css('left', '');
//    });
//};

//function moveRight() {
//    $('#slider ul').animate({
//        left: - slideWidth
//    }, 200, function () {
//        $('#slider ul li:first-child').appendTo('#slider ul');
//        $('#slider ul').css('left', '');
//    });
//};

//$('a.control_prev').click(function () {
//    moveLeft();
//});

//$('a.control_next').click(function () {
//    moveRight();
//});

//function initSlider() {
//    slideCount = $('#slider ul li').length;
//    slideWidth = $('#slider ul li').width();
//    slideHeight = $('#slider ul li').height();
//    sliderUlWidth = slideCount * slideWidth;

//    $('#slider').css({ width: slideWidth, height: slideHeight });

//    $('#slider ul').css({ width: sliderUlWidth, marginLeft: - slideWidth });

//    $('#slider ul li:last-child').prependTo('#slider ul');

//    function moveLeft() {
//        $('#slider ul').animate({
//            left: + slideWidth
//        }, 200, function () {
//            $('#slider ul li:last-child').prependTo('#slider ul');
//            $('#slider ul').css('left', '');
//        });
//    };

//    function moveRight() {
//        $('#slider ul').animate({
//            left: - slideWidth
//        }, 200, function () {
//            $('#slider ul li:first-child').appendTo('#slider ul');
//            $('#slider ul').css('left', '');
//        });
//    };

//    $('a.control_prev').click(function () {
//        moveLeft();
//    });

//    $('a.control_next').click(function () {
//        moveRight();
//    });
//}

//slider 2 end

//let slideIndex = 1;
//showSlides(slideIndex);

//// Next/previous controls
//function plusSlides(n) {
//    showSlides(slideIndex += n);
//}

//// Thumbnail image controls
//function currentSlide(n) {
//    showSlides(slideIndex = n);
//}

//function showSlides(n) {
//    let i;
//    let slides = document.getElementsByClassName("mySlides");
//    let dots = document.getElementsByClassName("dot");
//    if (n > slides.length) { slideIndex = 1 }
//    if (n < 1) { slideIndex = slides.length }
//    for (i = 0; i < slides.length; i++) {
//        slides[i].style.display = "none";
//    }
//    for (i = 0; i < dots.length; i++) {
//        dots[i].className = dots[i].className.replace(" active", "");
//    }
//    slides[slideIndex - 1].style.display = "block";
//    dots[slideIndex - 1].className += " active";
//}
