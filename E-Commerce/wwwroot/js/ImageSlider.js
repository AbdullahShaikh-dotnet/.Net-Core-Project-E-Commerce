const sliderInit = function () {
    const track = document.querySelector(".image-track");
    const prevButton = document.getElementById("prev-button");
    const nextButton = document.getElementById("next-button");
    const trackContainer = document.querySelector(".image-track-container");

    let mouseDownAt = 0;
    let prevPercentage = 0;
    let percentage = 0;
    let isDragging = false;
    let animationFrameId = null;
    let targetPercentage = 0;
    let currentPercentage = 0;
    const sensitivity = 0.5;
    const easing = 0.05;

    const animate = () => {
        currentPercentage += (targetPercentage - currentPercentage) * easing;
        track.style.transform = `translate(${currentPercentage}%, -50%)`;

        document.querySelectorAll(".image").forEach(img => {
            img.style.objectPosition = `${100 + currentPercentage}% center`;
        });

        if (Math.abs(targetPercentage - currentPercentage) > 0.1) {
            animationFrameId = requestAnimationFrame(animate);
        }
    };

    const handleMouseDown = (e) => {
        if (animationFrameId) cancelAnimationFrame(animationFrameId);
        animationFrameId = null;

        mouseDownAt = e.clientX;
        isDragging = true;
        currentPercentage = targetPercentage = prevPercentage;
        track.style.transition = "none";
        document.querySelectorAll(".image").forEach(img => img.style.transition = "none");
        e.preventDefault();
    };

    const handleMouseUp = () => {
        if (!isDragging) return;
        mouseDownAt = 0;
        prevPercentage = percentage;
        isDragging = false;
        targetPercentage = percentage;
        if (!animationFrameId) animationFrameId = requestAnimationFrame(animate);
    };

    const handleMouseMove = (e) => {
        if (!isDragging) return;
        const mouseDelta = mouseDownAt - e.clientX;
        const maxDelta = window.innerWidth / 2;
        const percentageDelta = (mouseDelta / maxDelta) * -100;
        const nextPercentageUnconstrained = prevPercentage + percentageDelta;
        percentage = Math.max(Math.min(nextPercentageUnconstrained, 0), -60);
        currentPercentage = targetPercentage = percentage;
        track.style.transform = `translate(${percentage}%, -50%)`;
        document.querySelectorAll(".image").forEach(img => {
            img.style.objectPosition = `${100 + percentage}% center`;
        });
    };

    const moveTrack = (direction) => {
        if (animationFrameId) cancelAnimationFrame(animationFrameId);
        animationFrameId = null;
        const moveAmount = direction === 'prev' ? 30 : -30;
        targetPercentage = Math.max(Math.min(targetPercentage + moveAmount, 0), -60);
        prevPercentage = percentage = targetPercentage;
        if (!animationFrameId) animationFrameId = requestAnimationFrame(animate);
    };

    track.addEventListener("mousedown", handleMouseDown);
    window.addEventListener("mouseup", handleMouseUp);
    window.addEventListener("mousemove", handleMouseMove);
    prevButton.addEventListener("click", () => moveTrack('prev'));
    nextButton.addEventListener("click", () => moveTrack('next'));
    track.addEventListener("contextmenu", (e) => e.preventDefault());
    window.addEventListener("mouseleave", handleMouseUp);
    currentPercentage = targetPercentage = 0;
    track.style.transform = `translate(0%, -50%)`;
};
