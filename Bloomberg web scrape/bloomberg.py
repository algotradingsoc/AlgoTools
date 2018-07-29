from urllib.request import urlopen as uReq
from bs4 import BeautifulSoup as soup


def scrape_bloomberg(file_name = "bloomberg_articles.csv"):
        my_url = "https://www.bloomberg.com/markets"
        uClient = uReq(my_url)
        page_html = uClient.read()
        uClient.close()

        page_soup = soup(page_html, "html.parser")
        articles= page_soup.findAll("article")
                             
        #File writting
        f = open(file_name, 'w')
        headers = "Title,Time,URL\n"
        f.write(headers)
        count = 0
        remove = 'Share on FacebookShare on Twitter'
        for article in articles:
                count +=1
                #print(count)
                link =  article.a["href"]
                full_url = "https://www.bloomberg.com" + link
                if article.time:
                        title = article.text.replace(article.time.text, "")
                        title = title.replace(remove, "")
                        if article.span:
                                title = title.replace(article.span.text, "")
                        time = article.time.text
                        f.write(title.replace(",", "|") + "," + time+ "," + full_url  + "\n")
                else:
                        title = article.text
                        if article.span:
                                title = title.replace(article.span.text, "")
                        title = title.replace(remove, "")
                        f.write(title.replace(",", "|") + "," + "," + full_url +  "\n")
        f.close()


if __name__ == '__main__':
        scrape_bloomberg()
