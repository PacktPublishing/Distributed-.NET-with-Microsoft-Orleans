# Distributed .NET with Microsoft Orleans

<a href="https://www.packtpub.com/product/distributed-net-with-microsoft-orleans/9781801818971"><img src="https://static.packt-cdn.com/products/9781801818971/cover/smaller" alt="Digital Marketing with Drupal" height="256px" align="right"></a>

This is the code repository for [Distributed .NET with Microsoft Orleans](https://www.packtpub.com/product/distributed-net-with-microsoft-orleans/9781801818971), published by Packt.

**Build robust and highly scalable distributed applications without worrying about complex programming patterns**

## What is this book about?
Building distributed applications in this modern era can be a tedious task as customers expect high availability, high performance, and improved resilience. With the help of this book, you'll discover how you can harness the power of Microsoft Orleans to build impressive distributed applications.

This book covers the following exciting features:
* Get to grips with the different cloud architecture patterns that can be leveraged for building distributed applications
* Manage state and build a custom storage provider
* Explore Orleans key design patterns and understand when to reuse them
* Work with different classes that are created by code generators in the Orleans framework
* Write unit tests for Orleans grains and silos and create mocks for different parts of the system
* Overcome traditional challenges of latency and scalability while building distributed applications

If you feel this book is for you, get your [copy](https://www.amazon.com/Distributed-NET-Microsoft-Orleans-applications-dp-1801818975/dp/1801818975/ref=mt_other?_encoding=UTF8&me=&qid=) today!


## Instructions and Navigations
All of the code is organized into folders. For example, Chapter04.

The code will look like the following:
```
public interface IHotelGrain :IGrainWithStringKey
{
Task<string> WelcomeGreetingAsync(string
guestName);
}

```

**Following is what you need for this book:**
This book is for .NET developers and software architects looking for a simplified guide for creating distributed applications, without worrying about complex programming patterns. Intermediate web developers who want to build highly scalable distributed applications will also find this book useful. A basic understanding of .NET Classic or .NET Core with C# and Azure will be helpful.

With the following software and hardware list you can run all code files present in the book (Chapter 1-11).

### Software and Hardware List
| Chapter | Software/Hardware required | OS required |
| -------- | ------------------------------------ | ----------------------------------- |
| 1-11 | C# | Windows Mac OS X and Linux |
| 1-11 | .NET 6 | Windows Mac OS X and Linux  |
| 1-11 | Visual Studio 2022 | Windows Mac OS X and Linux  |
| 1-11 | Microsoft Azure | Windows Mac OS X and Linux  |

We also provide a PDF file that has color images of the screenshots/diagrams used in this book. [Click here to download it](https://static.packt-cdn.com/downloads/9781801818971_ColorImages.pdf).


### Related products
* Microservices Communication in .NET Using gRPC [[Packt]](https://www.packtpub.com/product/microservices-communication-in-net-using-grpc/9781803236438) [[Amazon]](https://www.amazon.com/Microservices-Communication-NET-Using-gRPC/dp/1803236434)

* Enterprise Application Development with C# 9 and .NET 5 [[Packt]](https://www.packtpub.com/product/enterprise-application-development-with-c-9-and-net-5/9781800209442) [[Amazon]](https://www.amazon.com/Enterprise-Application-Development-NET-professional-grade/dp/1800209444)


## Get to Know the Authors

**Bhupesh Guptha Muthiyalu** 
 is a Microsoft Certified Professional and works at the company as a principal software engineering manager. He has 17+ years of software development experience on the .NET technology stack. His current role involves designing systems that are resilient to the iterations and changes required by the needs of enterprise businesses, validating architectural innovations, delivering solutions with high quality, managing the end-to-end ownership of products, and building diverse teams with capabilities to fulfill customer objectives. He is passionate about creating reusable components and identifying opportunities to make a product better.

**Suneel Kumar Kunani**
is a passionate developer who strives to learn something new every day. With over 17 years of experience in .NET and Microsoft technologies, he works on architecting and building mission-critical, highly scalable, and secure solutions at Microsoft. He loves to teach and preach the best practices in building distributed cloud solutions.


## Other books by the authors
* [Enterprise Application Development with C# 9 and .NET 5](https://www.packtpub.com/product/enterprise-application-development-with-c-9-and-net-5/9781800209442)
