//const sliderInit = function () {
//    const track = document.querySelector(".image-track");
//    const prevButton = document.getElementById("prev-button");
//    const nextButton = document.getElementById("next-button");
//    const trackContainer = document.querySelector(".image-track-container");


//    const slides = Array.from(track.querySelectorAll(".image-slide"));
//    const slideCount = slides.length;
//    const container = track.parentElement;


//    // Add image counter
//    const addImageCounter = () => {
//        const countIndicator = document.createElement('div');
//        countIndicator.className = 'absolute bottom-2 right-2 bg-white/80 text-xs text-gray-500 px-2 py-1 rounded-full';
//        countIndicator.textContent = `${slideCount} images`;
//        container.appendChild(countIndicator);
//    };

//    let mouseDownAt = 0;
//    let prevPercentage = 0;
//    let percentage = 0;
//    let isDragging = false;
//    let animationFrameId = null;
//    let targetPercentage = 0;
//    let currentPercentage = 0;
//    const sensitivity = 0.5;
//    const easing = 0.05;

//    const animate = () => {
//        currentPercentage += (targetPercentage - currentPercentage) * easing;
//        track.style.transform = `translate(${currentPercentage}%, -50%)`;

//        document.querySelectorAll(".image").forEach(img => {
//            img.style.objectPosition = `${100 + currentPercentage}% center`;
//        });

//        if (Math.abs(targetPercentage - currentPercentage) > 0.1) {
//            animationFrameId = requestAnimationFrame(animate);
//        }
//    };

//    const handleMouseDown = (e) => {
//        if (animationFrameId) cancelAnimationFrame(animationFrameId);
//        animationFrameId = null;

//        mouseDownAt = e.clientX;
//        isDragging = true;
//        currentPercentage = targetPercentage = prevPercentage;
//        track.style.transition = "none";
//        document.querySelectorAll(".image").forEach(img => img.style.transition = "none");
//        e.preventDefault();
//    };

//    const handleMouseUp = () => {
//        if (!isDragging) return;
//        mouseDownAt = 0;
//        prevPercentage = percentage;
//        isDragging = false;
//        targetPercentage = percentage;
//        if (!animationFrameId) animationFrameId = requestAnimationFrame(animate);
//    };

//    const handleMouseMove = (e) => {
//        if (!isDragging) return;
//        const mouseDelta = mouseDownAt - e.clientX;
//        const maxDelta = window.innerWidth / 2;
//        const percentageDelta = (mouseDelta / maxDelta) * -100;
//        const nextPercentageUnconstrained = prevPercentage + percentageDelta;
//        percentage = Math.max(Math.min(nextPercentageUnconstrained, 0), -60);
//        currentPercentage = targetPercentage = percentage;
//        track.style.transform = `translate(${percentage}%, -50%)`;
//        document.querySelectorAll(".image").forEach(img => {
//            img.style.objectPosition = `${100 + percentage}% center`;
//        });
//    };

//    const moveTrack = (direction) => {
//        if (animationFrameId) cancelAnimationFrame(animationFrameId);
//        animationFrameId = null;
//        const moveAmount = direction === 'prev' ? 30 : -30;
//        targetPercentage = Math.max(Math.min(targetPercentage + moveAmount, 0), -60);
//        prevPercentage = percentage = targetPercentage;
//        if (!animationFrameId) animationFrameId = requestAnimationFrame(animate);
//    };

//    track?.addEventListener("mousedown", handleMouseDown);
//    window.addEventListener("mouseup", handleMouseUp);
//    window.addEventListener("mousemove", handleMouseMove);
//    prevButton?.addEventListener("click", () => moveTrack('prev'));
//    nextButton?.addEventListener("click", () => moveTrack('next'));
//    track?.addEventListener("contextmenu", (e) => e.preventDefault());
//    window.addEventListener("mouseleave", handleMouseUp);
//    currentPercentage = targetPercentage = 0;
//    if (track) {
//        track.style.transform = `translate(0%, -50%)`;
//    }



//    // Initialize the slider
//    const init = () => {
//        addImageCounter();
//    };


//    init();
//};




const sliderInit = function () {
    const track = document.querySelector(".image-track");
    const prevButton = document.getElementById("prev-button");
    const nextButton = document.getElementById("next-button");
    const slides = track?.querySelectorAll(".image-slide") || [];

    // Early exit if no track found
    if (!track) return;

    // Calculate total width and max scroll
    const slideWidth = 150; // Width of each slide
    const slideGap = 16;    // Gap between slides (4px gap + borders)
    const totalSlides = slides.length;
    const totalWidth = totalSlides * (slideWidth + slideGap);
    const containerWidth = track.parentElement.clientWidth;

    // Calculate the maximum negative percentage based on total content
    const maxNegativePercentage = -((totalWidth / containerWidth * 100) - 100);

    // State variables
    let mouseDownAt = 0;
    let prevPercentage = 0;
    let percentage = 0;
    let isDragging = false;
    let startX = 0;
    let lastFrameTime = 0;
    let velocity = 0;
    const friction = 0.85;
    const minVelocity = 0.01;

    // Use transform3d for hardware acceleration
    const setTransform = (percentage) => {
        track.style.transform = `translate3d(${percentage}%, -50%, 0)`;

        // Update object position for images with throttling
        const images = track.querySelectorAll(".image");
        for (let i = 0; i < images.length; i++) {
            images[i].style.objectPosition = `${100 + percentage}% center`;
        }
    };

    // Smooth animation with velocity
    const animateWithVelocity = () => {
        if (Math.abs(velocity) < minVelocity) return;

        // Apply friction to slow down
        velocity *= friction;

        // Update percentage based on velocity
        percentage += velocity;

        // Constrain percentage
        if (percentage > 0) {
            percentage = 0;
            velocity = 0;
        } else if (percentage < maxNegativePercentage) {
            percentage = maxNegativePercentage;
            velocity = 0;
        }

        // Apply transform
        setTransform(percentage);
        prevPercentage = percentage;

        // Continue animation if velocity is significant
        if (Math.abs(velocity) >= minVelocity) {
            requestAnimationFrame(animateWithVelocity);
        }
    };

    // Smooth animation to target
    const animateToTarget = (targetPercentage, duration = 500) => {
        const startPercentage = percentage;
        const startTime = performance.now();
        const change = targetPercentage - startPercentage;

        const easeOutQuart = (t) => 1 - Math.pow(1 - t, 4); // Smoother easing

        const animate = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            const easedProgress = easeOutQuart(progress);

            percentage = startPercentage + change * easedProgress;
            setTransform(percentage);

            if (progress < 1) {
                requestAnimationFrame(animate);
            } else {
                prevPercentage = percentage;
            }
        };

        requestAnimationFrame(animate);
    };

    // Handle mouse events
    const handleMouseDown = (e) => {
        mouseDownAt = e.clientX;
        startX = e.clientX;
        isDragging = true;
        velocity = 0;

        // Record time for velocity calculation
        lastFrameTime = performance.now();

        // Prevent default behavior
        e.preventDefault();
    };

    const handleMouseMove = (e) => {
        if (!isDragging) return;

        // Calculate time delta for velocity
        const currentTime = performance.now();
        const deltaTime = currentTime - lastFrameTime;
        lastFrameTime = currentTime;

        // Calculate movement
        const currentX = e.clientX;
        const deltaX = currentX - startX;
        startX = currentX;

        // Calculate velocity (pixels per millisecond)
        if (deltaTime > 0) {
            velocity = deltaX / deltaTime * 5; // Scale for better feel
        }

        // Calculate percentage movement
        const mouseDelta = mouseDownAt - e.clientX;
        const maxDelta = window.innerWidth / 2;
        const percentageDelta = (mouseDelta / maxDelta) * -100;
        const nextPercentageUnconstrained = prevPercentage + percentageDelta;

        // Constrain percentage with elastic resistance at edges
        if (nextPercentageUnconstrained > 0) {
            percentage = nextPercentageUnconstrained * 0.3; // Elastic resistance
        } else if (nextPercentageUnconstrained < maxNegativePercentage) {
            const overscroll = nextPercentageUnconstrained - maxNegativePercentage;
            percentage = maxNegativePercentage + overscroll * 0.3; // Elastic resistance
        } else {
            percentage = nextPercentageUnconstrained;
        }

        // Apply transform
        setTransform(percentage);
    };

    const handleMouseUp = () => {
        if (!isDragging) return;
        isDragging = false;
        mouseDownAt = 0;

        // Snap back if overscrolled
        if (percentage > 0) {
            animateToTarget(0);
        } else if (percentage < maxNegativePercentage) {
            animateToTarget(maxNegativePercentage);
        } else if (Math.abs(velocity) > minVelocity) {
            // Continue with momentum if velocity is significant
            requestAnimationFrame(animateWithVelocity);
        }

        prevPercentage = percentage;
    };

    // Handle button navigation
    const moveTrack = (direction) => {
        // Calculate visible width
        const visibleWidth = track.parentElement.clientWidth;
        const visiblePercentage = (visibleWidth / totalWidth) * 100;

        // Move by 80% of visible width
        const moveAmount = direction === 'prev' ? visiblePercentage * 0.8 : -visiblePercentage * 0.8;
        let targetPercentage = percentage + moveAmount;

        // Constrain target
        targetPercentage = Math.max(Math.min(targetPercentage, 0), maxNegativePercentage);

        // Animate to target
        animateToTarget(targetPercentage);
    };

    // Touch events
    const handleTouchStart = (e) => {
        mouseDownAt = e.touches[0].clientX;
        startX = e.touches[0].clientX;
        isDragging = true;
        velocity = 0;
        lastFrameTime = performance.now();
    };

    const handleTouchMove = (e) => {
        if (!isDragging) return;

        // Calculate time delta for velocity
        const currentTime = performance.now();
        const deltaTime = currentTime - lastFrameTime;
        lastFrameTime = currentTime;

        // Calculate movement
        const currentX = e.touches[0].clientX;
        const deltaX = currentX - startX;
        startX = currentX;

        // Calculate velocity
        if (deltaTime > 0) {
            velocity = deltaX / deltaTime * 5;
        }

        // Calculate percentage movement
        const mouseDelta = mouseDownAt - e.touches[0].clientX;
        const maxDelta = window.innerWidth / 2;
        const percentageDelta = (mouseDelta / maxDelta) * -100;
        const nextPercentageUnconstrained = prevPercentage + percentageDelta;

        // Constrain percentage with elastic resistance
        if (nextPercentageUnconstrained > 0) {
            percentage = nextPercentageUnconstrained * 0.3;
        } else if (nextPercentageUnconstrained < maxNegativePercentage) {
            const overscroll = nextPercentageUnconstrained - maxNegativePercentage;
            percentage = maxNegativePercentage + overscroll * 0.3;
        } else {
            percentage = nextPercentageUnconstrained;
        }

        // Apply transform
        setTransform(percentage);

        // Prevent page scrolling
        e.preventDefault();
    };

    const handleTouchEnd = () => {
        if (!isDragging) return;
        isDragging = false;
        mouseDownAt = 0;

        // Snap back if overscrolled
        if (percentage > 0) {
            animateToTarget(0);
        } else if (percentage < maxNegativePercentage) {
            animateToTarget(maxNegativePercentage);
        } else if (Math.abs(velocity) > minVelocity) {
            // Continue with momentum
            requestAnimationFrame(animateWithVelocity);
        }

        prevPercentage = percentage;
    };

    // Add event listeners
    track.addEventListener("mousedown", handleMouseDown);
    window.addEventListener("mousemove", handleMouseMove);
    window.addEventListener("mouseup", handleMouseUp);

    track.addEventListener("touchstart", handleTouchStart, { passive: true });
    window.addEventListener("touchmove", handleTouchMove, { passive: false });
    window.addEventListener("touchend", handleTouchEnd);

    prevButton?.addEventListener("click", () => moveTrack('prev'));
    nextButton?.addEventListener("click", () => moveTrack('next'));

    // Prevent context menu
    track.addEventListener("contextmenu", (e) => e.preventDefault());

    // Handle window resize
    window.addEventListener("resize", () => {
        // Recalculate maxNegativePercentage on resize
        const containerWidth = track.parentElement.clientWidth;
        const newMaxNegativePercentage = -((totalWidth / containerWidth * 100) - 100);

        // Update maxNegativePercentage
        if (maxNegativePercentage !== newMaxNegativePercentage) {
            // If current percentage is beyond new limit, animate to new limit
            if (percentage < newMaxNegativePercentage) {
                animateToTarget(newMaxNegativePercentage);
            }
        }
    });

    // Initialize track position
    setTransform(0);

    // Add visual indicator for scrollable content
    if (totalWidth > containerWidth) {
        const scrollIndicator = document.createElement("div");
        scrollIndicator.className = "absolute bottom-2 right-2 bg-white/80 text-xs text-gray-500 px-2 py-1 rounded-full";
        scrollIndicator.textContent = `${totalSlides} images`;
        track.parentElement.appendChild(scrollIndicator);
    }
};

