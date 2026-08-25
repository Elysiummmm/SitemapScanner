# SitemapScanner

A small utility program written in C# to scan sitemaps and report on broken links. 
It runs a GET request on every URL listed in the sitemap.
Sitemap indexes also work and redirects get logged and verified as well.

## Usage
Just run the executable. There are some optional command line arguments listed below.

| Argument  | Description                                        | Example                                                |
|-----------|----------------------------------------------------|--------------------------------------------------------|
| `--url`   | Skip the URL prompt to provide a URL directly.     | `SitemapScanner --url https://example.com/sitemap.xml` |
| `--log`   | If provided, writes logs to the executable folder. | `SitemapScanner --log`                                 |