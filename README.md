# Windowed-Image-Processing-Program

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/ecff771d-e064-4a48-89fd-704c93ba6567)

The program is written in C# and features custom-designed algorithms for each function.
It does not directly utilize existing image processing algorithms, such as algorithms in OpenCV.

The included functions are as follows:

- [RGB Extraction & Transformation](#1)
- [Smooth Filter (Mean and Median)](#2)
- [Histogram Equalization](#3)
- [User-Defined Thresholding](#4)
- [Sobel Edge Detection](#5)
- [Edge Overlapping](#6)
- [Connected Component](#7)
- [Image Registration](#8)

---

<h2 id="1">RGB Extraction & Transformation</h2>

Extract the R, G, and B channels from the color image and transform it into a grayscale image.

Color extraction: Extract R, G, and B channels to each image

Color transformation: Change the color image to a grayscale image

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/70286b7a-8cba-4a20-ab08-550923dc5ee3) ![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/61acfd0d-710e-40f4-bdf8-7ed7fad08f9d)

**[⬆ back to top](#Windowed-Image-Processing-Program)**

<h2 id="2">Smooth Filter (Mean and Median)</h2>

Implement mean and median filters

Filter size: 3x3

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/bb3581de-26ce-47f2-b615-8b687cd28ce0)

<h2 id="3">Histogram Equalization</h2>

Implement histogram equalization: Show histogram of images before and after processing

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/3b86927a-afaf-4f73-b90a-3a439e288d6a)


<h2 id="4">User-Defined Thresholding</h2>

Given a threshold t. The intensity of a pixel that is higher than or equal to t will be set as black (255), otherwise set as white (0).

The threshold is input from the slider (1 ~ 255) on the interface.  ![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/46f846b5-f324-437d-808a-fc33402cc6d4)

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/79343402-4f7e-496a-a374-f9aa6cdfe58b)


<h2 id="5">Sobel Edge Detection</h2>

The program incorporates Sobel edge detection by applying it separately in the horizontal and vertical directions and subsequently combining the results.

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/72e5754f-6468-4271-af07-81ced35d378e)


<h2 id="6">Edge Overlapping</h2>

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/fa545757-9f1d-4ea5-96e3-2ec42e345ce6)


<h2 id="7">Connected Component</h2>

Connected Component: Count the number of connected regions in a binary image and paint it with different colors:

Input image : Foreground(black), background(white)

Using 8-adjacency

The pixel on the border of the image should be considered.
Adjacent regions are displayed in different colors.

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/fd58ce16-60ea-4f6f-9b98-321ca26398db)



<h2 id="8">Image Registration</h2>

![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/8d09077a-7f2a-48c6-b91f-fecdba11ac59)

Given two images A and B, B is a transformation of A by scaling and rotation. Register image B to image A.

1. The scaling factor 𝑠 of the registration

2. The rotation angle 𝜃(clockwise)

3. Evaluate the difference between image A and the registered image Intensity difference.

$$ D_{pixel}=\frac{1}{\left|I\right|}\sum_{p\in I}{|\widehat{i_p}-i_p|} $$

$$ I: Points\ in\ image $$

$$ i_p: Intensity\ of\ point\ p  $$  


![圖片](https://github.com/YeeHaoSu/Windowed-Image-Processing-Program/assets/90921571/5405d6c2-3b66-4bac-bb52-948cb3dc6182)


