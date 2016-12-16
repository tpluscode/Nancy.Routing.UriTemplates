![trail icon](https://raw.githubusercontent.com/tpluscode/Nancy.Routing.UriTemplates/master/assets/noun_27516.png)

# Nancy.Routing.UriTemplates [![Build status][av-badge]][build] [![NuGet version][nuget-badge]][nuget-link] [![codecov.io][cov-badge]][cov-link] [![codefactor][codefactor-badge]][codefactor-link]

Using [URI Templates aka RFC 6570](https://tools.ietf.org/html/rfc6570) for routing in Nancy. 

## Introduction

You can use URI Templates to define routes in Nancy.

``` c#
public class PeopleModule : Nancy.Routing.UriTemplates.UriTemplateModule
{
    public PeopleModule()
    {
        // by default you'll be creating typical routes
        Get("person/{id}", p => GetModel(p.id));
            
        // and URI Templates are opt-in
        using(Templates)
        {
            Get("people{;include}{/page}{?name}", p => GetPeople(p.include, p.page, p.name));
        }
    }
}
```

The second route will for example match a request to `people;include=hobbies,friends/4?name=Tomasz`

As you can see, URI Templates give more expressive syntax:

* variables can be extracted not only from segments but also other URI parts including query strings (`{?name}`) and path parameters (`{;include}`)
* all variables can have multiple values

Finally, URI Templates can also be used outside routing to mint identifiers. For example to used them as links in a REST API.

## More

See some discussion here: http://t-code.pl/blog/2016/11/Towards-server-side-routing-with-URI-Templates/

[The icon](https://thenounproject.com/term/trail/27516/) desiged by [Gabriele Debolini](http://thenounproject.com/gabriele.debolini/) from [The Noun Project](http://thenounproject.com/)

[av-badge]: https://ci.appveyor.com/api/projects/status/so0uk5kw89371b3f?svg=true
[build]: https://ci.appveyor.com/project/tpluscode78631/nancy-routing-uritemplates/branch/master
[nuget-badge]: https://badge.fury.io/nu/nancy.Routing.UriTemplates.svg
[nuget-link]: https://badge.fury.io/nu/nancy.Routing.UriTemplates
[cov-badge]: https://codecov.io/github/tpluscode/Nancy.Routing.UriTemplates/coverage.svg?branch=master
[cov-link]: https://codecov.io/github/tpluscode/Nancy.Routing.UriTemplates?branch=master
[codefactor-badge]: https://www.codefactor.io/repository/github/tpluscode/Nancy.Routing.UriTemplates/badge/master
[codefactor-link]: https://www.codefactor.io/repository/github/tpluscode/Nancy.Routing.UriTemplates/overview/master
