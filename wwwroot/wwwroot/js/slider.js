$(document).ready(function () {
    $('.owl-carousel').owlCarousel({
        items: 1,
        loop: true,
        margin: 10,
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
        },
        autoplay: true,
        autoplayTimeout: 4000,
    });
});