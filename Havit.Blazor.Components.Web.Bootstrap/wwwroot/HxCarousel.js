﻿export function SlideTo(id, index) {
    GetCarousel(id).to(index);
}

export function Previous(id) {
    GetCarousel(id).prev();
}

export function Next(id) {
    GetCarousel(id).next();
}

export function Cycle(id) {
    GetCarousel(id).cycle();
}

export function Pause(id) {
    GetCarousel(id).pause();
}

export function Dispose(id) {
    GetCarousel(id).dispose();
}

export function GetCarousel(id) {
    var carouselElement = document.getElementById(id);
    return bootstrap.Carousel.getInstance(carouselElement)
}

export function SetInterval(id, interval) {
    GetCarousel(id).interval = interval;
}

var HxCarouselDotnetObjectReference = undefined;

export function AddEventListeners(id, hxCarouselDotnetObjectReference) {
    let carouselElement = document.getElementById(id);
    HxCarouselDotnetObjectReference = hxCarouselDotnetObjectReference;

    carouselElement.addEventListener('slide.bs.carousel', OnSlide);
    carouselElement.addEventListener('slid.bs.carousel', OnSlid);
}

function OnSlide() {
    HxCarouselDotnetObjectReference.invokeMethodAsync('HxCarousel_HandleSlide');
}

function OnSlid() {
    HxCarouselDotnetObjectReference.invokeMethodAsync('HxCarousel_HandleSlid');
}