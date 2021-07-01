
$(window).on('load' , function () {
    
    $('.loader').fadeOut(1000);
    
    new WOW().init();
    
    $('body').addClass('o-auto');
    
})

$(document).ready(function () { 
    $('.default').on('click', function(e) {
        e.preventDefault();
    });
 
    // Start Navbar 
    $('.cart-overlay').fadeOut();
   $('.open-cartside').on('click', function () {
      event.preventDefault();
       $('.cart-div').addClass('cart-open');
       $('.cart-overlay').fadeIn();
   });
   $('.cart-overlay').on('click', function () {
       $('.cart-div').removeClass('cart-open');
       $(this).fadeOut();
   });

    $('.overlay').fadeOut();
    
    $(".mob-collaps").on('click', function() {
        $(this).parent().find(".nav-links > ul").toggleClass('nav-open');

        $('.overlay').fadeToggle();

        $(this).find("span:first-child").toggleClass('rotate');
        $(this).find("span:nth-child(2)").toggleClass('none');
        $(this).find("span:nth-child(3)").toggleClass('rotate2');
    });

    $(".overlay").on('click', function() {
        $(".nav-links ul").removeClass('nav-open');
        $(this).fadeOut();

        $(".mob-collaps span:first-child").removeClass('rotate');
        $(".mob-collaps span:nth-child(2)").removeClass('none');
        $(".mob-collaps span:nth-child(3)").removeClass('rotate2');
    }); 
    
    $(".top-nav a").each(
        function() {
            if (window.location.href.includes($(this).attr('href'))) {
                $(this).parents('li ').addClass("active");
             }
        }
    );
       
   
   $('.header-slider .owl-carousel  ').owlCarousel({
            
    margin: 0,
       autoplay: false,
    loop: true,
    nav: false,
    dots:true,
 
     responsive: {
    0: {
    items: 1
    },
    600: {
    items: 1
    },
    1000: {
    items: 1
    }
    }
    });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
      });
     
      //$('#counter1').each(function(){
      //    var qtyval = parseInt($(this).find('#qty-val1').text());
      //    $('#qty-up1').click(function(event){
      //        event.preventDefault();
      //        qtyval=qtyval+1;
      //        $('#qty-val1').text(qtyval);
      //        $('#num').val(qtyval);
      //    });
      //    $('#qty-down1').click(function(event){
      //        event.preventDefault();
      //        qtyval=qtyval-1;
      //        if(qtyval>0){
      //            $('#qty-val1').text(qtyval);
      //            $('#num').val(qtyval);
      //        }else{
      //            qtyval=0;
      //            $('#qty-val1').text(qtyval);
      //            $('#num').val('');
      //        }
      //    });
      //});

      $('.input-img').change(function (event) {
        for (var one = 0; one < event.target.files.length; one++) {
        $(this).parents('.imgs-block').find('.upload-area').append('<div class="uploaded-block" data-count-order="' + one + '"><img src="' + URL.createObjectURL(event.target.files[one]) + '"><button class="close"> <i class="fa fa-times"></i></button></div>');
        }
        });
 
        $('.imgs-block').on('click', '.close',function (){
        $(this).parents('.uploaded-block').remove();
        });





    
});
/* Star Rating */
var $star_rating = $('.star-rating .fa');

var SetRatingStar = function () {
    return $star_rating.each(function () {
        if (parseInt($star_rating.siblings('input.rating-value').val()) >= parseInt($(this).data('rating'))) {
            return $(this).removeClass('fa-star-o').addClass('fa-star');
        } else {
            return $(this).removeClass('fa-star').addClass('fa-star-o');
        }
    });
};

$star_rating.on('click', function () {
    $star_rating.siblings('input.rating-value').val($(this).data('rating'));
    return SetRatingStar();
});

SetRatingStar();

/* End Rating */


$(".product-box .icon-fav").on("click", function () {

    $(this).toggleClass("active");

});